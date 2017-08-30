using DotCanal.Driver.Utils;
using System;
using System.IO;

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

        public override void FromBytes(byte[] data) { }

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
        public override byte[] ToBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(Command);
                    bw.Flush();
                    ByteHelper.WriteUnsignedIntLittleEndian(BinlogPosition, ms);
                    Int16 binlog_flags = 0;
                    binlog_flags |= BINLOG_SEND_ANNOTATE_ROWS_EVENT;
                    bw.Write(binlog_flags);
                    bw.Write(0x00);
                    bw.Flush();
                    ByteHelper.WriteUnsignedIntLittleEndian(SlaveServerId, ms);

                    if(!string.IsNullOrEmpty(BinlogFileName))
                    {
                        bw.Write(BinlogFileName);
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
