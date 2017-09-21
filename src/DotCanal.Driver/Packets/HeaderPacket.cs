using System;
using System.Text;

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

        public void FromBytes(MySqlPacket data)
        {
            if (data == null || data.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(data));
            data.Position = 0;
            PacketBodyLength = data.Read3ByteInt();
            PacketSequenceNumber = data.ReadByte();
        }

        public MySqlPacket ToBytes()
        {
            var packet = new MySqlPacket(Encoding.UTF8);
            packet.WriteInteger(PacketBodyLength, 3);
            packet.WriteByte(PacketSequenceNumber);

            return packet;
        }
    }
}
