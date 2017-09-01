namespace DotCanal.Driver.Packets.Server
{
    public class DataPacket : CommandPacket
    {
        public override void FromBytes(MySqlPacket data) { }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
