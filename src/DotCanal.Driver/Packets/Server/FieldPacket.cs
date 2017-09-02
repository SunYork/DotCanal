using System;
using System.Collections.Generic;
using System.Text;

namespace DotCanal.Driver.Packets.Server
{
    public class FieldPacket : PacketWithHeaderPacket
    {
        public string Catalog { get; set; }
        public string DataBase { get; set; }
        public string Table { get; set; }
        public string OriginalTable { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public int Character { get; set; }
        public long Length { get; set; }
        public byte Type { get; set; }
        public int Flags { get; set; }
        public byte Decimals { get; set; }

    }
}
