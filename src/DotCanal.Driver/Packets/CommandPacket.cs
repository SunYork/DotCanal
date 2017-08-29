namespace DotCanal.Driver.Packets
{
    public abstract class CommandPacket : IPacket
    {
        public byte Command { get; set; }

        public abstract void FromBytes(byte[] data);

        public abstract byte[] ToBytes();
    }
}
