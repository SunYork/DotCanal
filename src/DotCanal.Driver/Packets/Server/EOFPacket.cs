﻿using System.Text;

namespace DotCanal.Driver.Packets.Server
{
    public class EOFPacket : PacketWithHeaderPacket
    {
        public byte FieldCount { get; set; }
        public int WarningCount { get; set; }
        public int StatusFlag { get; set; }

        public override void FromBytes(MySqlPacket data)
        {
            //1.read field count
            FieldCount = data.ReadByte();
            //2.read warning count
            WarningCount = data.ReadInteger(2);
            //3.read status flag
            StatusFlag = data.ReadInteger(2);
        }

        public override MySqlPacket ToBytes()
        {
            var packet = new MySqlPacket(Encoding.UTF8);
            packet.WriteByte(FieldCount);
            packet.WriteInteger(WarningCount, 2);
            packet.WriteInteger(StatusFlag, 2);
            return packet;
        }
    }
}
