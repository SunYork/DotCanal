using DotCanal.Driver.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotCanal.Driver.Packets.Client
{
    public class ClientAuthenticationPacket : PacketWithHeaderPacket
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public byte CharsetNumber { get; set; }
        public string DatabaseName { get; set; }
        public int ServerCapabilities { get; set; }
        public byte[] ScrumbleBuff { get; set; }

        public override void FromBytes(byte[] data)
        {

        }

        /// <summary>
        /// 字节             名称
        /// -----            ----
        /// 4                client_flags
        /// 4                max_packet_size
        /// 1                charset_number
        /// 23               过滤，一直为0x00..
        /// n                user
        /// n                scramble_buff (1 + x bytes)
        /// n                databasename（可选）
        /// </summary>
        public override byte[] ToBytes()
        {
            using (var ms = new MemoryStream())
            {
                /* 1.写入 client_flags
                 * CLIENT_LONG_PASSWORD CLIENT_LONG_FLAG CLIENT_PROTOCOL_41
                 * CLIENT_INTERACTIVE CLIENT_TRANSACTIONS CLIENT_SECURE_CONNECTION
                 */
                ByteHelper.WriteUnsignedIntLittleEndian(1 | 4 | 512 | 8192 | 32768, ms);

                // 2. 写入 mac_packet_size 
                ByteHelper.WriteUnsignedIntLittleEndian(MSC.MAX_PACKET_LENGTH, ms);
                // 3. 写入 charset_number
                ms.WriteByte(CharsetNumber);
                // 4. 写入 过滤
                var filter = new byte[23];
                ms.Write(filter, 0, filter.Length);
                // 5. 写入 user
                ByteHelper.WriteNullTerminatedString(UserName, ms);
                // 6. 写入 scramble_buff
                if(string.IsNullOrEmpty(Password))
                {
                    ms.WriteByte(0x00);
                }
                else
                {

                }
            }
        }
    }
}
