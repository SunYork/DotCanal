﻿using DotCanal.Driver.MySqlClient;
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
        public byte[] ScrumbleBuff { get; set; }

        public override void FromBytes(MySqlPacket data) { }

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
        public override MySqlPacket ToBytes()
        {
            var packet = new MySqlPacket(Encoding.UTF8);
            packet.WriteInteger((int)(ClientFlags.LONG_PASSWORD | ClientFlags.LONG_FLAG
                | ClientFlags.PROTOCOL_41 | ClientFlags.INTERACTIVE | ClientFlags.TRANSACTIONS
                | ClientFlags.SECURE_CONNECTION), 4);
            packet.WriteInteger(MSC.MAX_PACKET_LENGTH, 4);
            packet.WriteByte(CharsetNumber);
            packet.Write(new byte[23]);
            //5.写入user
            packet.WriteString(UserName);
            //6.写入 scramble_buff
            if(string.IsNullOrEmpty(Password))
            {
                packet.WriteByte(0x00);
            }
            else
            {
                byte[] encryptedPassword = MySQLPasswordEncrypter.Scramble411(Password, ScrumbleBuff);
                packet.Write(encryptedPassword);
            }

            if (!string.IsNullOrEmpty(DatabaseName))
            {
                packet.WriteString(DatabaseName);
            }

            return packet;
        }
    }
}
