using System;
using System.IO;
using System.Text;

namespace DotCanal.Driver.Utils
{
    public static class ByteHelper
    {
        public const long NULL_LENGTH = -1;

        public static byte[] ReadNullTerminatedBytes(byte[] data, int index)
        {
            using (var outStream = new MemoryStream())
            {
                for(int i = index; i < data.Length; i++)
                {
                    byte item = data[i];
                    if(item == MSC.NULL_TERMINATED_STRING_DELIMITER)
                    {
                        break;
                    }
                    outStream.WriteByte(item);
                }
                return outStream.ToArray();
            }
        }

        public static void WriteNullTerminatedString(string str, Stream outstream)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            outstream.Write(strBytes, 0, strBytes.Length);
            outstream.WriteByte(MSC.NULL_TERMINATED_STRING_DELIMITER);
        }

        public static void WriteNullTerminated(byte[] data, Stream outstream)
        {
            outstream.Write(data, 0, data.Length);
            outstream.WriteByte(MSC.NULL_TERMINATED_STRING_DELIMITER);
        }

        public static byte[] ReadFixedLengthBytes(byte[] data, int index, int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(data, index, bytes, 0, length);
            return bytes;
        }

        public static long ReadUnsignedIntLittleEndian(byte[] data, int index)
        {
            long result = (long)(data[index] & 0xFF) | (long)((data[index + 1] & 0xFF) << 8)
                | (long)((data[index + 2] & 0xFF) << 16) | (long)((data[index + 3] & 0xFF) << 24);
            return result;
        }

        public static long ReadUnsignedLongLittleEndian(byte[] data, int index)
        {
            long accumulation = 0;
            int position = index;
            for (int shifyBy = 0; shifyBy < 64; shifyBy += 8)
            {
                accumulation |= (long)((data[position++] & 0xFF) << shifyBy);
            }
            return accumulation;
        }

        public static int ReadUnsignedShortLittleEndian(byte[] data, int index)
        {
            int result = (data[index] & 0xFF) | ((data[index + 1] & 0xFF) << 8);
            return result;
        }

        public static int ReadUnsignedMediumLittleEndian(byte[] data, int index)
        {
            int result = (data[index] & 0xFF) | ((data[index + 1] & 0xFF) << 8) | ((data[index + 2] & 0xFF) << 16);
            return result;
        }

        public static long ReadLengthCodedBinary(byte[] data, int index)
        {
            int firstByte = data[index] & 0xFF;
            switch (firstByte)
            {
                case 251:
                    return NULL_LENGTH;
                case 252:
                    return ReadUnsignedShortLittleEndian(data, index + 1);
                case 253:
                    return ReadUnsignedMediumLittleEndian(data, index + 1);
                case 254:
                    return ReadUnsignedLongLittleEndian(data, index + 1);
                default:
                    return firstByte;
            }
        }

        public static byte[] ReadBinaryCodedLengthBytes(byte[] data, int index)
        {
            using (var outStream = new MemoryStream())
            {
                outStream.WriteByte(data[index]);
                byte[] buffer = null;
                int value = data[index] & 0xFF;
                if (value == 251)
                {
                    buffer = new byte[0];
                }
                else if (value == 252)
                {
                    buffer = new byte[2];
                }
                else if (value == 253)
                {
                    buffer = new byte[3];
                }
                else if (value == 254)
                {
                    buffer = new byte[8];
                }
                if (buffer != null)
                {
                    Array.Copy(data, index + 1, buffer, 0, buffer.Length);
                    outStream.Write(buffer, 0, buffer.Length);
                }
                return outStream.ToArray();
            }
        }

        public static void WriteUnsignedIntLittleEndian(long data, Stream outstream)
        {
            outstream.WriteByte((byte)(data & 0xFF));
            outstream.WriteByte((byte)(data >> 8));
            outstream.WriteByte((byte)(data >> 16));
            outstream.WriteByte((byte)(data >> 24));
        }

        public static void WriteUnsignedShortLittleEndian(int data, Stream outstream)
        {
            outstream.WriteByte((byte)(data & 0xFF));
            outstream.WriteByte((byte)((data >> 8) & 0xFF));
        }

        public static void WriteUnsignedMediumLittleEndian(int data, Stream outstream)
        {
            outstream.WriteByte((byte)(data & 0xFF));
            outstream.WriteByte((byte)((data >> 8) & 0xFF));
            outstream.WriteByte((byte)((data >> 16) & 0xFF));
        }

        public static void WriteBinaryCodedLengthBytes(byte[] data, Stream outstream)
        {
            if(data.Length < 252)
            {
                outstream.WriteByte((byte)data.Length);
            }
            else if(data.Length < (1 << 16))
            {
                outstream.WriteByte(252);
                WriteUnsignedShortLittleEndian(data.Length, outstream);
            }
            else if(data.Length < (1 << 24))
            {
                outstream.WriteByte(253);
                WriteUnsignedMediumLittleEndian(data.Length, outstream);
            }
            else
            {
                outstream.WriteByte(254);
                WriteUnsignedIntLittleEndian(data.Length, outstream);
            }
            outstream.Write(data, 0, data.Length);
        }

        public static void WriteFixedLengthBytes(byte[] data, int index, int length, Stream outstream)
        {
            for (int i = index; i < index; i++)
            {
                outstream.WriteByte(data[i]);
            }
        }

        public static void WriteFixedLengthBytesFromStart(byte[] data, int length, Stream outstream)
        {
            WriteFixedLengthBytes(data, 0, length, outstream);
        }
    }
}
