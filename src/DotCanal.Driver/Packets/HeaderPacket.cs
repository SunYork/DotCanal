using System;

namespace DotCanal.Driver.Packets
{
    /// <summary>
    /// 偏移  长度  说明
    ///  0     3    包主体大小
    ///  3     1    包片段数
    ///  
    /// </summary>
    public class HeaderPacket : IPacket
    {
        /// <summary>
        /// 此字段表示头部后面的数据包长度，排除这头部的4个字节。
        /// </summary>
        public int PacketBodyLength { get; set; }

        public byte PacketSequenceNumber { get; set; }

        public void FromBytes(byte[] data)
        {
            if (data == null || data.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(data));

            PacketBodyLength = (data[0] & 0xFF) | ((data[1] & 0xFF) << 8) | ((data[2] & 0xFF) << 16);
            PacketSequenceNumber = data[3];
        }

        public byte[] ToBytes()
        {
            byte[] data = new byte[4];
            data[0] = (byte)(PacketBodyLength & 0xFF);
            data[1] = (byte)(PacketBodyLength >> 8);
            data[2] = (byte)(PacketBodyLength >> 16);
            data[3] = PacketSequenceNumber;
            return data;
        }
    }
}
