using DotCanal.Driver.Packets.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotCanal.Driver.Utils
{
    public class BinlogDumpCommandBuilder
    {
        public static BinlogDumpCommandPacket Build(string binlogfile, long position, long slaveId)
        {
            BinlogDumpCommandPacket command = new BinlogDumpCommandPacket();
            command.BinlogPosition = position;
            if (!string.IsNullOrEmpty(binlogfile))
                command.BinlogFileName = binlogfile;
            command.SlaveServerId = slaveId;
            return command;
        }
    }
}
