using DotCanal.Common.Log;
using DotCanal.Driver.Socket;
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
                    Disconnect();
                    throw ex;
                }
            }
            else
            {
                _logger.LogError("the channel can't be connected twice.");
            }
        }

        public Task Disconnect()
        {

        }

        private void Negotiate(SocketChannel channel)
        {

        }
    }
}
