using System.Text;

namespace DotCanal.Driver.Packets.Client
{
    public class QueryCommandPacket : CommandPacket
    {
        public string QueryString { get; set; }

        public QueryCommandPacket()
        {
            Command = 0x03;
        }

        public override void FromBytes(MySqlPacket data)
        {
        }

        public override MySqlPacket ToBytes()
        {
            var packet = new MySqlPacket(Encoding.UTF8);
            packet.WriteByte(Command);
            packet.WriteStringNoNull(QueryString);

            return packet;
        }
    }
}
