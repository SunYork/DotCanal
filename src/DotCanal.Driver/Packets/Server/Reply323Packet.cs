using System.Text;

namespace DotCanal.Driver.Packets.Server
{
    public class Reply323Packet : PacketWithHeaderPacket
    {
        public string Seed { get; set; }

        public override void FromBytes(MySqlPacket data)
        {

        }

        public override MySqlPacket ToBytes()
        {
            var packet = new MySqlPacket(Encoding.UTF8);
            if(Seed == null)
            {
                packet.WriteByte(0x00);
            }
            else
            {
                packet.WriteString(Seed);
            }

            return packet;
        }
    }
}
