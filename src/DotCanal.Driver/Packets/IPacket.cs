namespace DotCanal.Driver.Packets
{
    /// <summary>
    /// 数据包基础接口
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// 从字节包恢复
        /// </summary>
        void FromBytes(MySqlPacket data);

        /// <summary>
        /// 将对象转换为字节包
        /// </summary>
        MySqlPacket ToBytes();
    }
}
