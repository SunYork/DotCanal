using DotCanal.Driver.MySqlClient;

namespace DotCanal.Driver.Packets.Server
{
    public class OKPacket : PacketWithHeaderPacket
    {
        public byte Header { get; set; }
        public byte AffectedRows { get; set; }
        public byte InsertId { get; set; }
        public ServerStatusFlags ServerStatus { get; set; }
        public int WarningCount { get; set; }
        public string Message { get; set; }

        public override void FromBytes(MySqlPacket data)
        {
            Header = data.ReadByte();
            AffectedRows = data.ReadByte();
            InsertId = data.ReadByte();
            //4. read server status
            ServerStatus = (ServerStatusFlags)data.ReadInteger(2);
            //5. read warning count
            WarningCount = data.ReadInteger(2);
            //6. read message
            Message = data.ReadLenString();
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
