using System.Text;

namespace DotCanal.Driver.Packets.Client
{
    public class QuitCommandPacket : CommandPacket
    {
        public static byte[] QUIT = new byte[] { 1, 0, 0, 0, 1 };

        public QuitCommandPacket()
        {
            Command = 0x01;
        }

        public override void FromBytes(MySqlPacket data)
        {

        }

        public override MySqlPacket ToBytes()
        {
            var packet = new MySqlPacket(Encoding.UTF8);
            packet.WriteByte(Command);
            packet.Write(QUIT);

            return packet;
        }
    }
}
