using DotCanal.Common.Log;
using DotCanal.Driver.Packets;
using DotCanal.Driver.Packets.Client;
using DotCanal.Driver.Packets.Server;
using DotCanal.Driver.Socket;
using DotCanal.Driver.Utils;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DotCanal.Driver
{
    public class MysqlConnector
    {
        private static readonly ILogger _logger = ApplicationLogging.CreateLogger<MysqlConnector>();
        private SocketChannel _channel;

        public EndPoint Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public byte CharsetNumber { get; set; } = 33;
        public string DefaultSchema { get; set; } = "retl";
        public int SoTimeout { get; set; } = 300 * 1000;
        public int ReceiveBufferSize = 16 * 1024;
        public int SendBufferSize = 16 * 1024;

        public bool Dumping = false;
        public long ConnectionId { get; set; } = -1;

        public ConnectionState ConnectionState { get; private set; }

        public MysqlConnector() { }

        public MysqlConnector(EndPoint address, string username, string password)
        {
            Address = address;
            UserName = username;
            Password = password;
        }

        public async Task ConnectAsync()
        {
            if (ConnectionState == ConnectionState.Closed)
            {
                try
                {
                    _channel = await SocketChannelPool.OpenAsync(Address);
                    _logger.LogInformation($"Connect MysqlConnection to {Address}");
                    Negotiate(_channel);
                }
                catch(Exception ex)
                {
                    await DisconnectAsync();
                    throw ex;
                }
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

                if (Dumping && ConnectionId >= 0)
                {
                    MysqlConnector connector = null;

                }
            }
            return Task.CompletedTask;
        }

        private void Negotiate(SocketChannel channel)
        {
            var header = PacketManager.ReadHeader(channel, 4);
            byte[] body = PacketManager.ReadBytes(channel, header.PacketBodyLength);
            int bodyCode = (int)((body[0] << 24) | 0xFF << 16 | 0xFF << 8 | 0xFF);
            if(bodyCode < 0)
            {
                if(bodyCode == -1)
                {
                    ErrorPacket error = new ErrorPacket();
                    error.FromBytes(body);
                    throw new IOException($"Handshake exception:{error.ToString()}");
                }
                else if(bodyCode == -2)
                {
                    throw new IOException($"Unexpected EOF packet at handshake phase.");
                }
                else
                {
                    throw new IOException($"Unpexpected packet with field_count={bodyCode}");
                }
            }
            HandshakeInitializationPacket handshakePacket = new HandshakeInitializationPacket();
            handshakePacket.FromBytes(body);
            ConnectionId = handshakePacket.ThreadId;

            _logger.LogInformation("Handshake initialization packet received, prepare the client authentication packet to send");

            ClientAuthenticationPacket clientAuth = new ClientAuthenticationPacket();
            clientAuth.CharsetNumber = CharsetNumber;
            clientAuth.UserName = UserName;
            clientAuth.Password = Password;
            clientAuth.ServerCapabilities = handshakePacket.ServerCapabilities;
            clientAuth.DatabaseName = DefaultSchema;
            clientAuth.ScrumbleBuff = JoinAndCreateScrumbleBuff(handshakePacket);

            byte[] clientAuthPkgBody = clientAuth.ToBytes();
            HeaderPacket h = new HeaderPacket();
            h.PacketBodyLength = clientAuthPkgBody.Length;
            h.PacketSequenceNumber = (byte)(header.PacketSequenceNumber + 1);

            PacketManager.WritePkg(channel, h.ToBytes(), clientAuthPkgBody);
            _logger.LogInformation("Client Authentication Packet is sent out.");

            header = null;
            header = PacketManager.ReadHeader(channel, 4);
            body = null;
            body = PacketManager.ReadBytes(channel, header.PacketBodyLength);

            if (body == null)
                throw new ArgumentNullException(nameof(body));

            bodyCode = (int)body[0];
            if(bodyCode < 0)
            {
                if(bodyCode == -1)
                {
                    ErrorPacket err = new ErrorPacket();
                    err.FromBytes(body);
                    throw new IOException($"Error When doing Client Authentioncation:{err.ToString()}");
                }
                else if(bodyCode == -2)
                {
                    throw new IOException($"Unexpected EOF packet at Client Authentication.");
                }
                else
                {
                    throw new IOException($"Unpexpected Packet With field_count{bodyCode}");
                }
            }
        }

        private byte[] JoinAndCreateScrumbleBuff(HandshakeInitializationPacket handshakePacket)
        {
            byte[] dest = new byte[handshakePacket.Seed.Length + handshakePacket.RestOfScrambleBuff.Length];
            handshakePacket.Seed.CopyTo(dest, 0);
            handshakePacket.RestOfScrambleBuff.CopyTo(dest, handshakePacket.Seed.Length);
            return dest;
        }
    }
}
