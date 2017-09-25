namespace DotCanal.Driver.Packets.Server
{
    public class ResultSetHeaderPacket : PacketWithHeaderPacket
    {
        public long ColumnCount { get; set; }
        public long Extra { get; set; }

        public override void FromBytes(MySqlPacket data)
        {
            ColumnCount = data.ReadFieldLength();
            if (data.Length > 0)
            {
                Extra = data.ReadFieldLength();
            }
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
