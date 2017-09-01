namespace DotCanal.Driver.Packets
{
    public abstract class CommandPacket : IPacket
    {
        public byte Command { get; set; }

        public abstract void FromBytes(MySqlPacket data);

        public abstract MySqlPacket ToBytes();
    }
}
