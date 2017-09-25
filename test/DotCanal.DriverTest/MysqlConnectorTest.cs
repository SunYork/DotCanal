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
            MysqlConnector connector = new MysqlConnector(new IPEndPoint(IPAddress.Parse("192.168.0.243"), 3306), "root", "123456");
            try
            {
                await connector.ConnectAsync();
                var result = connector.Query("SHOW VARIABLES LIKE '%char%';");
                result = connector.Query("SELECT * FROM test.food");
                var ok = connector.Update("UPDATE test.food SET Name = '123测试123小龙虾' WHERE Id = '69c09f4e-2f69-4dda-8820-11edbf6ab28a';");
            }
            finally
            {
                connector.Quit();
                await connector.DisconnectAsync();
            }
        }
    }
}
