using DotCanal.Driver;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DotCanal.DriverTest
{
    public class MysqlConnectorTest
    {
        [Fact]
        public async Task TestQuery()
        {
            MysqlConnector connector = new MysqlConnector(new IPEndPoint(IPAddress.Parse("192.168.0.242"), 3306), "root", "5802486");
            await connector.ConnectAsync();
        }
    }
}
