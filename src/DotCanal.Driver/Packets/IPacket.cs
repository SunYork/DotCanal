namespace DotCanal.Driver.Packets
{
    /// <summary>
    /// 数据包基础接口
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// 接收原始字节
        /// </summary>
        void FromBytes(byte[] data);

        /// <summary>
        /// 将对象转换为字节
        /// </summary>
        byte[] ToBytes();
    }
}
