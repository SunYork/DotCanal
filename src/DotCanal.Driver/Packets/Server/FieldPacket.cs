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
        public int Filter { get; set; }
        public byte Decimals { get; set; }
        public string Definition { get; set; }

        /// <summary>
        /// 字节               名称
        /// -----               -----
        ///  n                   catalog
        ///  n                   database
        ///  n                   table
        ///  n                   org_table
        ///  n                   name
        ///  n                   org_name
        ///  1                   filter
        ///  2                   charsetnr
        ///  4                   length
        ///  1                   type
        ///  2                   flags
        ///  1                   decimals
        ///  2                   filter
        ///  n                   detault
        /// </summary>
        public override void FromBytes(MySqlPacket data)
        {
            //1. read catelog
            Catalog = data.ReadLenString();
            //2. read database
            DataBase = data.ReadLenString();
            //3. read table
            Table = data.ReadLenString();
            //4. read originaltable
            OriginalTable = data.ReadLenString();
            //5. read name
            Name = data.ReadLenString();
            //6. read originalname
            OriginalName = data.ReadLenString();
            //7. read character
            Character = data.ReadInteger(2);
            //8. read length
            Length = data.ReadInteger(4);
            //7. read type
            Type = data.ReadByte();
            //8. read flags
            Flags = data.ReadInteger(2);
            //9. read decimals
            Decimals = data.ReadByte();
            //10. read filter
            Filter = data.ReadInteger(2);
            //11. read definition
            Definition = data.ReadLenString();
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
