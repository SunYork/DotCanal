namespace DotCanal.Driver.Packets
{
    public abstract class PacketWithHeaderPacket : IPacket
    {
        public HeaderPacket Header { get; set; }

        protected PacketWithHeaderPacket() { }

        protected PacketWithHeaderPacket(HeaderPacket header)
        {
            Header = header;
        }

        public abstract void FromBytes(MySqlPacket data);

        public abstract MySqlPacket ToBytes();
    }
}
