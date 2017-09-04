namespace DotCanal.Driver.Packets.Server
{
    public class OKPacket : PacketWithHeaderPacket
    {
        public byte FieldCount { get; set; }
        public byte[] AffectedRows { get; set; }
        public byte[] InsertId { get; set; }
        public int ServerStatus { get; set; }
        public int WarningCount { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// 字节                 名称
        /// ----                ------
        ///  1                  field_count
        ///  1-9                affected_rows
        ///  1-9                insert_id
        ///  2                  server_status
        ///  2                  warning_count
        ///  n                  message
        /// </summary>
        public override void FromBytes(MySqlPacket data)
        {
            //1. read field count
            FieldCount = data.ReadByte();
            //2. read affected rows
            data.Read(AffectedRows, 0, (int)data.ReadFieldLength());
            //3. read insert id
            data.Read(InsertId, 0, (int)data.ReadFieldLength());
            //4. read server status
            ServerStatus = data.ReadInteger(2);
            //5. read warning count
            WarningCount = data.ReadInteger(2);
            //6. read message
            Message = data.ReadString();
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
