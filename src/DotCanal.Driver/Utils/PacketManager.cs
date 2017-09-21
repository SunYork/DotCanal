using DotCanal.Driver.Packets;
using DotCanal.Driver.Socket;
using System.Text;

namespace DotCanal.Driver.Utils
{
    public static class PacketManager
    {
        public static HeaderPacket ReadHeader(SocketChannel channel,int len)
        {
            HeaderPacket header = new HeaderPacket();
            MySqlPacket packet = new MySqlPacket(Encoding.UTF8);
            packet.Write(channel.Read(len));
            header.FromBytes(packet);

            return header;
        }

        public static byte[] ReadBytes(SocketChannel channel, int len)
        {
            return channel.Read(len);
        }

        public static void WritePkg(SocketChannel channel, params byte[][] srcs)
        {
            channel.WriteChannel(srcs);
        }

        public static void WriteBody(SocketChannel channel, byte[] body)
        {
            WriteBody(channel, body, 0);
        }

        public static void WriteBody(SocketChannel channel, byte[] body, byte packetSeqNumber)
        {
            HeaderPacket header = new HeaderPacket();
            header.PacketBodyLength = body.Length;
            header.PacketSequenceNumber = packetSeqNumber;
            channel.WriteChannel(header.ToBytes().Buffer, body);
        }
    }
}
