using DotCanal.Common.Exceptions;
using DotCanal.Common.Log;
using DotCanal.Driver.MySqlClient;
using DotCanal.Driver.Packets;
using DotCanal.Driver.Packets.Client;
using DotCanal.Driver.Packets.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotCanal.Driver
{
    public class MysqlConnector
    {
        private static readonly ILogger _logger = ApplicationLogging.CreateLogger<MysqlConnector>();
        private MySqlStream _channel;

        public IPEndPoint Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public byte CharsetNumber { get; set; } = 33;
        public string DefaultSchema { get; set; } = "test";
        public int SoTimeout { get; set; } = 300 * 1000;
        public int ReceiveBufferSize = 16 * 1024;
        public int SendBufferSize = 16 * 1024;

        public bool Dumping = false;
        public long ConnectionId { get; set; } = -1;

        public ConnectionState ConnectionState { get; private set; }

        public MysqlConnector() { }

        public MysqlConnector(IPEndPoint address, string username, string password)
        {
            Address = address;
            UserName = username;
            Password = password;
        }

        public async Task ConnectAsync()
        {
            if (ConnectionState == ConnectionState.Closed)
            {
                Stream baseStream = null;
                try
                {
                    var client = new TcpClient(AddressFamily.InterNetwork);
                    await client.ConnectAsync(Address.Address, Address.Port);
                    _logger.LogInformation($"Connect MysqlConnection to {Address}");
                    baseStream = client.GetStream();
                    _channel = new MySqlStream(baseStream, Encoding.UTF8, false);
                    Negotiate(_channel);
                }
                catch(System.Security.SecurityException)
                {
                    throw;
                }
                catch(Exception ex)
                {
                    throw new DotCanalException("无法连接到指定位置");
                }
                finally
                {
                    await DisconnectAsync();
                }
                if (baseStream == null)
                    throw new DotCanalException("无法连接到指定位置");
            }
            else
            {
                _logger.LogError("the channel can't be connected twice.");
            }
        }

        public async Task ReconnectAsync()
        {
            await DisconnectAsync();
            await ConnectAsync();
        }

        public Task DisconnectAsync()
        {
            if (ConnectionState != ConnectionState.Closed)
            {
                try
                {
                    if (_channel != null)
                    {
                        _channel.Close();
                    }
                    _logger.LogInformation("Disconnect MysqlConnection to {0}", Address);
                }
                catch (Exception e)
                {
                    throw new IOException($"Diconnect {Address} failure", e);
                }
            }
            return Task.CompletedTask;
        }

        public void Quit()
        {
            QuitCommandPacket quit = new QuitCommandPacket();
            _channel.SendPacket(quit.ToBytes());
        }

        public ResultSetPacket Query(string queryString)
        {
            _channel.SequenceByte = 0;

            QueryCommandPacket cmd = new QueryCommandPacket();
            cmd.QueryString = queryString;
            _channel.SendPacket(cmd.ToBytes());

            var result = _channel.ReadPacket();

            ResultSetHeaderPacket rsHeader = new ResultSetHeaderPacket();
            rsHeader.FromBytes(result);

            var fields = new List<FieldPacket>();
            for (int i = 0; i < rsHeader.ColumnCount; i++)
            {
                var fp = new FieldPacket();
                fp.FromBytes(_channel.ReadPacket());
                fields.Add(fp);
            }

            ReadEofPacket();

            var rowData = new List<RowDataPacket>();
            while(true)
            {
                var body = _channel.ReadPacket();
                if (body.Buffer[0] == 254)
                    break;
                var rowDataPacket = new RowDataPacket();
                rowDataPacket.FromBytes(body);
                rowData.Add(rowDataPacket);
            }

            var resultSet = new ResultSetPacket();
            resultSet.FieldDescriptors.AddRange(fields);
            foreach(var r in rowData)
            {
                resultSet.FieldValues.AddRange(r.Columns);
            }
            resultSet.SourceAddress = Address;

            return resultSet;
        }

        public OKPacket Update(string updateString)
        {
            _channel.SequenceByte = 0;

            var cmd = new QueryCommandPacket();
            cmd.QueryString = updateString;
            _channel.SendPacket(cmd.ToBytes());

            _logger.LogDebug("Read update result.");

            var result = _channel.ReadPacket();

            var okPacket = new OKPacket();
            okPacket.FromBytes(result);

            return okPacket;
        }

        private void ReadEofPacket()
        {
            var eofPacket = _channel.ReadPacket();
            if (eofPacket.Buffer[0] != 254)
                throw new DotCanalException($"EOF Packet is expected, but packet with field_count=[{eofPacket.Buffer[0]}] is found.");
        }

        private void Negotiate(MySqlStream channel)
        {
            var body = channel.ReadPacket();
            HandshakeInitializationPacket handshakePacket = new HandshakeInitializationPacket();
            handshakePacket.FromBytes(body);
            ConnectionId = handshakePacket.ThreadId;

            _logger.LogInformation("Handshake initialization packet received, prepare the client authentication packet to send");

            ClientAuthenticationPacket clientAuth = new ClientAuthenticationPacket();
            clientAuth.CharsetNumber = CharsetNumber;
            clientAuth.UserName = UserName;
            clientAuth.Password = Password;
            clientAuth.DatabaseName = DefaultSchema;
            clientAuth.ScrumbleBuff = handshakePacket.EncryptionSeed;

            var clientAuthPkgBody = clientAuth.ToBytes();
            channel.SendPacket(clientAuthPkgBody);
            _logger.LogInformation("Client Authentication Packet is sent out.");

            channel.ReadPacket();
        }
    }
}
