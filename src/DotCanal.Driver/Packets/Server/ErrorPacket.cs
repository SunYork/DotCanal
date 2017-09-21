namespace DotCanal.Driver.Packets.Server
{
    public class ErrorPacket : PacketWithHeaderPacket
    {
        public byte FieldCount { get; set; }
        public int ErrorNumber { get; set; }
        public byte SqlStateMarker { get; set; }
        public byte[] SqlState { get; set; } = new byte[5];
        public string Message { get; set; }

        /// <summary>
        /// 字节                  名称
        /// -----                 ----
        ///  1                      field_count, always = 0xff
        ///  2                      error
        ///  1                      sqlstate marker,always #
        ///  5                      sqlstate
        ///  n                      message
        /// </summary>
        public override void FromBytes(MySqlPacket data)
        {
            data.Position = 0;

            //1. read field count
            FieldCount = data.ReadByte();
            //2. read error no
            ErrorNumber = data.ReadInteger(2);
            //3. read marker
            //SqlStateMarker = data.ReadByte();
            //4. read sqlstate
            //data.Read(SqlState, 0, 5);
            //5. read message
            Message = data.ReadString();
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
