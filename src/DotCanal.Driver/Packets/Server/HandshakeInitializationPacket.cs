using DotCanal.Driver.Utils;

namespace DotCanal.Driver.Packets.Server
{
    public class HandshakeInitializationPacket : PacketWithHeaderPacket
    {
        public byte ProtocolVersion { get; set; } = MSC.DEFAULT_PROTOCOL_VERSION;
        public string ServerVersion { get; set; }
        public long ThreadId { get; set; }
        public byte[] Seed { get; set; }
        public byte Filler { get; set; }
        public int ServerCapabilities { get; set; }
        public byte ServerCharsetNumber { get; set; }
        public int ServerStatus { get; set; }
        public byte[] RestOfScrambleBuff { get; set; }

        public HandshakeInitializationPacket() { }

        public HandshakeInitializationPacket(HeaderPacket header)
            : base(header) { }

        /// <summary>
        /// 字节                  名称
        /// ----                 ------
        ///  1                    protocol_version
        ///  n                    server_version
        ///  4                    thread_id
        ///  8                    scramble_buff
        ///  1                    filler
        ///  2                    server_capabilities
        ///  1                    server_language
        ///  2                    server_status
        ///  13                   filler
        ///  13                   rest of scramble_buff
        /// </summary>
        public override void FromBytes(MySqlPacket data)
        {
            //1. read protocolversion
            ProtocolVersion = data.ReadByte();
            //2. read server_version
            ServerVersion = data.ReadString();
            //3. read thread_id
            ThreadId = data.ReadInteger(4);
            //4. read scramble_buff
            data.Read(Seed, 0, 8);
            //5. read filler
            Filler = data.ReadByte();
            //6. read server_capabilities
            ServerCapabilities = data.ReadInteger(2);
            //7. read server_language
            ServerCharsetNumber = data.ReadByte();
            //8. read server_status
            ServerStatus = data.ReadInteger(2);
            //9. bypass filtered bytes
            data.Position += 13;
            //10. read rest of scramble_buff
            data.Read(RestOfScrambleBuff, 0, 12);

            //Packet规定最后13个byte是剩下的scrumble,
            //但实际上最后一个字节是0,不应该包含在内.
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
