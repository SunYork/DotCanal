using System.Text;

namespace DotCanal.Driver.Packets.Client
{
    public class BinlogDumpCommandPacket : CommandPacket
    {
        public const int BINLOG_DUMP_NON_BLOCK = 1;
        public const short BINLOG_SEND_ANNOTATE_ROWS_EVENT = 2;

        public long BinlogPosition { get; set; }
        public long SlaveServerId { get; set; }
        public string BinlogFileName { get; set; }

        public BinlogDumpCommandPacket()
        {
            Command = 0x12;
        }

        public override void FromBytes(MySqlPacket data) { }

        /// <summary>
        /// 字节         名称
        ///  1            命令
        ///  n            命令参数
        ///  ---------------------
        ///  4            binlog起始坐标
        ///  2            binlog标志
        ///  4            server id
        ///  n            binlog文件名
        /// </summary>
        public override MySqlPacket ToBytes()
        {
            var packet = new MySqlPacket(Encoding.UTF8);
            packet.WriteByte(Command);
            packet.WriteInteger(BinlogPosition, 4);
            packet.WriteInteger(BINLOG_SEND_ANNOTATE_ROWS_EVENT, 2);
            packet.WriteByte(0x00);
            packet.WriteInteger(SlaveServerId, 4);
            if (!string.IsNullOrEmpty(BinlogFileName))
            {
                packet.WriteStringNoNull(BinlogFileName);
            }

            return packet;
        }
    }
}
