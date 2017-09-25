using DotCanal.Driver.MySqlClient;
using DotCanal.Driver.Utils;

namespace DotCanal.Driver.Packets.Server
{
    public class HandshakeInitializationPacket : PacketWithHeaderPacket
    {
        public byte ProtocolVersion { get; set; } = MSC.DEFAULT_PROTOCOL_VERSION;
        public string ServerVersion { get; set; }
        public long ThreadId { get; set; }
        public ClientFlags Flags { get; set; }
        public int ServerCharsetNumber { get; set; }
        public ServerStatusFlags ServerStatus { get; set; }
        public string AuthenticationMethod { get; set; }
        public byte[] EncryptionSeed { get; set; }

        public HandshakeInitializationPacket() { }

        public HandshakeInitializationPacket(HeaderPacket header)
            : base(header) { }

        public override void FromBytes(MySqlPacket data)
        {
            ProtocolVersion = data.ReadByte();
            ServerVersion = data.ReadString();
            ThreadId = data.ReadInteger(4);

            byte[] seedPart1 = data.ReadStringAsBytes();

            if (data.HasMoreData)
            {
                Flags = (ClientFlags)data.ReadInteger(2);
            }
            ServerCharsetNumber = (int)data.ReadByte();
            ServerStatus = (ServerStatusFlags)data.ReadInteger(2);
            var serverCapsHigh = (uint)data.ReadInteger(2);
            Flags |= (ClientFlags)(serverCapsHigh << 16);
            data.Position += 11;

            byte[] seedPart2 = data.ReadStringAsBytes();

            if ((Flags & ClientFlags.PLUGIN_AUTH) != 0)
            {
                AuthenticationMethod = data.ReadString();
            }
            else
            {
                AuthenticationMethod = "mysql_native_password";
            }

            EncryptionSeed = new byte[seedPart1.Length + seedPart2.Length];
            seedPart1.CopyTo(EncryptionSeed, 0);
            seedPart2.CopyTo(EncryptionSeed, seedPart1.Length);
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
