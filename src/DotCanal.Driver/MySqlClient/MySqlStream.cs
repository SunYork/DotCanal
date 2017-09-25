using DotCanal.Common.Exceptions;
using DotCanal.Driver.Packets;
using System;
using System.IO;
using System.Text;

namespace DotCanal.Driver.MySqlClient
{
    internal class MySqlStream
    {
        private byte _sequenceByte;
        private int _maxBlockSize;
        private ulong _maxPacketSize;
        private byte[] _packetHeader = new byte[4];
        private MySqlPacket _packet;
        private TimedStream _timedStream;
        private Stream _inStream;
        private Stream _outStream;

        internal Stream BaseStream
        {
            get
            {
                return _timedStream;
            }
        }

        public MySqlStream(Encoding encoding)
        {
            _maxPacketSize = ulong.MaxValue;
            _maxBlockSize = Int32.MaxValue;
            _packet = new MySqlPacket(encoding);
        }

        public MySqlStream(Stream baseStream, Encoding encoding, bool compress)
          : this(encoding)
        {
            _timedStream = new TimedStream(baseStream);
            Stream stream;
            if (compress)
                stream = new CompressedStream(_timedStream);
            else
                stream = _timedStream;

            _inStream = stream;
            _outStream = stream;
        }

        public void Close()
        {
            _outStream.Dispose();
            _inStream.Dispose();
            _timedStream.Close();
        }

        public Encoding Encoding
        {
            get { return _packet.Encoding; }
            set { _packet.Encoding = value; }
        }

        public void ResetTimeout(int timeout)
        {
            _timedStream.ResetTimeout(timeout);
        }

        public byte SequenceByte
        {
            get { return _sequenceByte; }
            set { _sequenceByte = value; }
        }

        public int MaxBlockSize
        {
            get { return _maxBlockSize; }
            set { _maxBlockSize = value; }
        }

        public ulong MaxPacketSize
        {
            get { return _maxPacketSize; }
            set { _maxPacketSize = value; }
        }

        /// <summary>
        /// ReadPacket is called by NativeDriver to start reading the next
        /// packet on the stream.
        /// </summary>
        public MySqlPacket ReadPacket()
        {
            LoadPacket();

            if (_packet.Buffer[0] == 0xff)
            {
                _packet.ReadByte();  // read off the 0xff

                int code = _packet.ReadInteger(2);
                string msg = String.Empty;

                msg = _packet.ReadString(Encoding.UTF8);

                if (msg.StartsWith("#", StringComparison.Ordinal))
                {
                    msg.Substring(1, 5);  /* state code */
                    msg = msg.Substring(6);
                }
                throw new DotCanalException(msg, code);
            }
            return _packet;
        }

        /// <summary>
        /// Reads the specified number of bytes from the stream and stores them at given 
        /// offset in the buffer.
        /// Throws EndOfStreamException if not all bytes can be read.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="buffer"> Array to store bytes read from the stream </param>
        /// <param name="offset">The offset in buffer at which to begin storing the data read from the current stream. </param>
        /// <param name="count">Number of bytes to read</param>
        internal static void ReadFully(Stream stream, byte[] buffer, int offset, int count)
        {
            int numRead = 0;
            int numToRead = count;
            while (numToRead > 0)
            {
                int read = stream.Read(buffer, offset + numRead, numToRead);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }
                numRead += read;
                numToRead -= read;
            }
        }

        /// <summary>
        /// LoadPacket loads up and decodes the header of the incoming packet.
        /// </summary>
        public void LoadPacket()
        {
            try
            {
                _packet.Length = 0;
                int offset = 0;
                while (true)
                {
                    ReadFully(_inStream, _packetHeader, 0, 4);
                    _sequenceByte = (byte)(_packetHeader[3] + 1);
                    int length = (int)(_packetHeader[0] + (_packetHeader[1] << 8) +
                        (_packetHeader[2] << 16));

                    // make roo for the next block
                    _packet.Length += length;
                    ReadFully(_inStream, _packet.Buffer, offset, length);
                    offset += length;

                    // if this block was < maxBlock then it's last one in a multipacket series
                    if (length < _maxBlockSize) break;
                }
                _packet.Position = 0;
            }
            catch (IOException ioex)
            {
                throw new DotCanalException("读取流失败", true, ioex);
            }
        }

        public void SendPacket(MySqlPacket packet)
        {
            byte[] buffer = packet.Buffer;
            int length = packet.Position - 4;

            if ((ulong)length > _maxPacketSize)
                throw new DotCanalException("查询过大");

            int offset = 0;
            do
            {
                int lenToSend = length > _maxBlockSize ? _maxBlockSize : length;
                buffer[offset] = (byte)(lenToSend & 0xff);
                buffer[offset + 1] = (byte)((lenToSend >> 8) & 0xff);
                buffer[offset + 2] = (byte)((lenToSend >> 16) & 0xff);
                buffer[offset + 3] = _sequenceByte++;

                _outStream.Write(buffer, offset, lenToSend + 4);
                _outStream.Flush();
                length -= lenToSend;
                offset += lenToSend;
            } while (length > 0);
        }

        public void SendEntirePacketDirectly(byte[] buffer, int count)
        {
            buffer[0] = (byte)(count & 0xff);
            buffer[1] = (byte)((count >> 8) & 0xff);
            buffer[2] = (byte)((count >> 16) & 0xff);
            buffer[3] = _sequenceByte++;
            _outStream.Write(buffer, 0, count + 4);
            _outStream.Flush();
        }
    }
}