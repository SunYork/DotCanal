using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotCanal.DbSync
{
    /// <summary>
    /// 可考虑使用BinaryReader和BinaryWriter改写
    /// </summary>
    public class LogBuffer
    {
        protected byte[] Buffer { get; set; }
        protected int Origin { get; set; }
        protected int Limit { get; set; }
        protected int Position { get; set; }

        protected LogBuffer() { }

        public LogBuffer(byte[] buffer, int origin, int limit)
        {
            if (origin + limit > buffer.Length)
                throw new ArgumentOutOfRangeException($"capacity excceed: {origin + limit}");

            Buffer = buffer;
            Origin = origin;
            Position = origin;
            Limit = limit;
        }

        public LogBuffer Duplicate(int pos, int len)
        {
            if (pos + len > Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {pos + len}");

            int off = Origin + pos;
            byte[] buf = new byte[len];
            Array.Copy(Buffer, pos, buf, 0, len);
            return new LogBuffer(buf, 0, len);
        }

        public LogBuffer Duplicate(int len)
        {
            if (Position + len > Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position + len - Origin}");

            var buf = new byte[len];
            Array.Copy(Buffer, Position, buf, 0, len);
            Position += len;
            return new LogBuffer(buf, 0, len);
        }

        public LogBuffer Duplicate()
        {
            var buf = new byte[Limit];
            Array.Copy(Buffer, Origin, buf, 0, Limit);
            return new LogBuffer(buf, 0, Limit);
        }

        public int Capacity()
        {
            return Buffer.Length;
        }

        public int GetPosition()
        {
            return Position - Origin;
        }

        public LogBuffer GetPosition(int newPosition)
        {
            if (newPosition > Limit || newPosition < 0)
                throw new ArgumentOutOfRangeException($"limit excceed:{newPosition}");

            Position = Origin + newPosition;
            return this;
        }

        public LogBuffer Forward(int len)
        {
            if (Position + len > Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position + len - Origin}");

            Position += len;
            return this;
        }

        public LogBuffer Consume(int len)
        {
            if (Limit > len)
            {
                Limit -= len;
                Origin += len;
                Position = Origin;
                return this;
            }
            else if (Limit == len)
            {
                Limit = 0;
                Origin = 0;
                Position = 0;
                return this;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"limit excceed: {len}");
            }
        }

        public LogBuffer Rewind()
        {
            Position = Origin;
            return this;
        }

        public int GetLimit()
        {
            return Limit;
        }

        public LogBuffer GetLimit(int newLimit)
        {
            if (Origin + newLimit > Buffer.Length)
                throw new ArgumentOutOfRangeException($"capacity excceed: {Origin + newLimit}");

            Limit = newLimit;
            return this;
        }

        public int Remaining()
        {
            return Limit + Origin - Position;
        }

        public bool HasRemaining()
        {
            return Position < Limit + Origin;
        }

        public sbyte GetInt8(int pos)
        {
            if (pos >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {pos}");

            return Convert.ToSByte(Buffer[Origin + pos]);
        }

        public sbyte GetInt8()
        {
            if (Position >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin}");

            return Convert.ToSByte(Buffer[Position++]);
        }

        public byte GetUint8(int pos)
        {
            if (pos >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {pos}");

            return Buffer[Origin + pos];
        }

        public int GetUint8()
        {
            if (Position >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin}");

            return Buffer[Position++];
        }

        public Int16 GetInt16(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 1 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: { (pos < 0 ? pos : pos + 1) }");

            return BitConverter.ToInt16(Buffer, newPos);
        }

        public Int16 GetInt16()
        {
            if (Position + 1 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 1}");

            int pos = Position;
            Position += 2;
            return BitConverter.ToInt16(Buffer, pos);
        }

        public UInt16 GetUint16(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 1 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 1)}");

            return BitConverter.ToUInt16(Buffer, newPos);
        }

        public UInt16 GetUint16()
        {
            if (Position + 1 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 1}");

            int pos = Position;
            Position += 2;
            return BitConverter.ToUInt16(Buffer, pos);
        }

        public int GetBeInt16(int pos)
        {
            int newPos = Origin + pos;
            if (pos + 1 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"liit excceed: {(pos < 0 ? pos : pos + 1)}");

            return (0xFF & Buffer[newPos + 1] | (Buffer[newPos] << 8));
        }

        public int GetBeInt16()
        {
            if (Position + 1 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 1}");

            return (Buffer[Position++] << 8) | (0xFF & Buffer[Position++]);
        }

        public uint GetBeUint16(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 1 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 1)}");

            return Convert.ToUInt16((0xFF & Buffer[newPos + 1]) | ((0xFF & Buffer[newPos]) << 8));
        }

        public uint GetBeUint16()
        {
            if (Position + 1 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 1}");

            return Convert.ToUInt16(((0xFF & Buffer[Position++]) << 8) | (0xFF & Buffer[Position++]));
        }

        public int GetInt24(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 2 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 2)}");

            return (0xFF & Buffer[Position]) | ((0xFF & Buffer[Position + 1]) << 8) | ((Buffer[Position + 2]) << 16);
        }

        public int GetInt24()
        {
            if (Position + 2 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 2}");

            return (0xFF & Buffer[Position++]) | ((0xFF & Buffer[Position++]) << 8) | ((Buffer[Position++]));
        }

        public int GetBeInt24(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 2 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : (pos + 2))}");

            return (0xFF & Buffer[newPos + 2]) | ((0xFF & Buffer[newPos + 1]) << 8) | (Buffer[newPos] << 16);
        }

        public int GetBeInt24()
        {
            if (Position + 2 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 2}");

            return ((Buffer[Position++]) << 16) | ((0xFF & Buffer[Position++]) << 8) | (0xFF & Buffer[Position++]);
        }

        public uint GetUint24(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 2 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 2)}");

            return Convert.ToUInt32((0xFF & Buffer[newPos]) | ((0xFF & Buffer[newPos + 1]) << 8) | ((0xFF & Buffer[newPos + 2]) << 16));
        }

        public uint GetUint24()
        {
            if (Position + 2 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 2}");

            return Convert.ToUInt32((0xFF & Buffer[Position++]) | ((0xFF & Buffer[Position++]) << 8) | ((0xFF & Buffer[Position++]) << 16));
        }

        public uint GetBeUint24(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 2 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 2)}");

            return Convert.ToUInt32((0xFF & Buffer[newPos + 2]) | ((0xFF & Buffer[newPos + 1]) << 8) | ((0xFF & Buffer[newPos])) << 16);
        }

        public uint GetBeUint24()
        {
            if (Position + 2 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position + Origin + 2}");

            return Convert.ToUInt32(((0xFF & Buffer[Position++]) << 16) | ((0xFF & Buffer[Position++]) << 8) | (0xFF & Buffer[Position++]));
        }

        public int GetInt32(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 3 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 3)}");

            return BitConverter.ToInt32(Buffer, Position);
        }

        public int GetBeInt32(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 3 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 3)}");

            return (0xFF & Buffer[newPos + 3]) | ((0xFF & Buffer[newPos + 3]) << 8) | ((0xFF & Buffer[newPos + 1]) << 16) & ((0xFF & Buffer[newPos]) << 24);
        }

        public int GetInt32()
        {
            if (Position + 3 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {(Position - Origin + 3)}");

            int pos = Position;
            Position += 4;
            return BitConverter.ToInt32(Buffer, pos);
        }

        public int GetBeInt32()
        {
            if (Position + 3 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 3}");

            return ((Buffer[Position++]) << 24) | ((0xFF & Buffer[Position++]) << 16) | ((0xFF & Buffer[Position++]) << 8) | (0xFF & Buffer[Position++]);
        }

        public uint GetUint32(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 3 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 3)}");

            return BitConverter.ToUInt32(Buffer, newPos);
        }

        public uint GetBeUint32(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 3 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 3)}");

            return Convert.ToUInt32((0xFF & Buffer[newPos + 3]) | ((0xFF & Buffer[newPos + 2]) << 8) | ((0xFF & Buffer[newPos + 1]) << 16) | ((0xFF & Buffer[newPos]) << 24));
        }

        public uint GetUint32()
        {
            if (Position + 3 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {(Position - Origin + 3)}");

            Position += 4;
            return BitConverter.ToUInt32(Buffer, Position);
        }

        public uint GetBeUint32()
        {
            if (Position + 3 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {(Position - Origin + 3)}");

            return Convert.ToUInt32(((0xFF & Buffer[Position++]) << 24) | ((0xFF & Buffer[Position++]) << 16) | ((0xFF & Buffer[Position++]) << 8) | (0xFF & Buffer[Position++]));
        }

        public long GetUlong40(int pos)
        {
            int newPost = Origin + pos;

            if (pos + 4 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 4)}");

            return ((long)(0xff & Buffer[newPost])) | ((long)(0xff & Buffer[newPost + 1]) << 8)
                | ((long)(0xff & Buffer[newPost + 2]) << 16) | ((long)(0xff & Buffer[newPost + 3]) << 24)
                | ((long)(0xff & Buffer[newPost + 4]) << 32);
        }

        public long GetUlong40()
        {
            if (Position + 4 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 4}");

            return ((long)(0xff & Buffer[Position++])) | ((long)(0xff & Buffer[Position++]) << 8)
                | ((long)(0xff & Buffer[Position++]) << 16) | ((long)(0xff & Buffer[Position++]) << 24)
                | ((long)(0xff & Buffer[Position++]) << 32);
        }

        public long GetBeUlong40(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 4 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limit excceed: {(pos < 0 ? pos : pos + 4)}");

            return ((long)(0xff & Buffer[newPos + 4])) | ((long)(0xff & Buffer[newPos + 3]) << 8)
                | ((long)(0xff & Buffer[newPos + 2]) << 16) | ((long)(0xff & Buffer[newPos + 1]) << 24)
                | ((long)(0xff & Buffer[newPos]) << 32);
        }

        public long GetBeUlong40()
        {
            if (Position + 4 >= Origin + Limit)
                throw new ArgumentOutOfRangeException($"limit excceed: {Position - Origin + 4}");

            return ((long)(0xff & Buffer[Position++]) << 32) | ((long)(0xff & Buffer[Position++]) << 24)
                | ((long)(0xff & Buffer[Position++]) << 16) | ((long)(0xff & Buffer[Position++]) << 8)
                | ((long)(0xff & Buffer[Position++]));
        }

        public long GetLong48(int pos)
        {
            int newPos = Origin + pos;

            if (pos + 5 >= Limit || pos < 0)
                throw new ArgumentOutOfRangeException($"limite excceed: {(pos < 0 ? pos : pos + 5)}");

            return ((long)(0xff & Buffer[newPos])) | ((long)(0xff & Buffer[newPos + 1]) << 8)
                | ((long)(0xff & Buffer[newPos + 2]) << 16) | ((long)(0xff & Buffer[newPos + 3]) << 24)
                | ((long)(0xff & Buffer[newPos + 4]) << 32) | ((long)(Buffer[newPos + 5]) << 40);
        }
    }
}
