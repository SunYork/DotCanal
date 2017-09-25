using DotCanal.Driver.Common;
using System;
using System.IO;

namespace DotCanal.Driver.MySqlClient
{
    internal class TimedStream : Stream
    {
        readonly Stream _baseStream;

        int _timeout;
        int _lastReadTimeout;
        int _lastWriteTimeout;
        readonly LowResolutionStopwatch _stopwatch;

        internal bool IsClosed { get; private set; }

        enum IOKind
        {
            Read,
            Write
        };

        public TimedStream(Stream baseStream)
        {
            _baseStream = baseStream;
            _timeout = baseStream.CanTimeout ? baseStream.ReadTimeout : System.Threading.Timeout.Infinite;
            IsClosed = false;
            _stopwatch = new LowResolutionStopwatch();
        }

        private bool ShouldResetStreamTimeout(int currentValue, int newValue)
        {
            if (!_baseStream.CanTimeout) return false;
            if (newValue == System.Threading.Timeout.Infinite
                && currentValue != newValue)
                return true;
            if (newValue > currentValue)
                return true;
            return currentValue >= newValue + 100;
        }

        private void StartTimer(IOKind op)
        {

            int streamTimeout;

            if (_timeout == System.Threading.Timeout.Infinite)
                streamTimeout = System.Threading.Timeout.Infinite;
            else
                streamTimeout = _timeout - (int)_stopwatch.ElapsedMilliseconds;

            if (op == IOKind.Read)
            {
                if (ShouldResetStreamTimeout(_lastReadTimeout, streamTimeout))
                {
                    _baseStream.ReadTimeout = streamTimeout;
                    _lastReadTimeout = streamTimeout;
                }
            }
            else
            {
                if (ShouldResetStreamTimeout(_lastWriteTimeout, streamTimeout))
                {
                    _baseStream.WriteTimeout = streamTimeout;
                    _lastWriteTimeout = streamTimeout;
                }
            }

            if (_timeout == System.Threading.Timeout.Infinite)
                return;

            _stopwatch.Start();
        }

        private void StopTimer()
        {
            if (_timeout == System.Threading.Timeout.Infinite)
                return;

            _stopwatch.Stop();

            if (_stopwatch.ElapsedMilliseconds > _timeout)
            {
                ResetTimeout(System.Threading.Timeout.Infinite);
                throw new TimeoutException("Timeout in IO operation");
            }
        }
        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => _baseStream.CanSeek;

        public override bool CanWrite => _baseStream.CanWrite;

        public override void Flush()
        {
            try
            {
                StartTimer(IOKind.Write);
                _baseStream.Flush();
                StopTimer();
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override long Length => _baseStream.Length;

        public override long Position
        {
            get
            {
                return _baseStream.Position;
            }
            set
            {
                _baseStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                StartTimer(IOKind.Read);
                int retval = _baseStream.Read(buffer, offset, count);
                StopTimer();
                return retval;
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override int ReadByte()
        {
            try
            {
                StartTimer(IOKind.Read);
                int retval = _baseStream.ReadByte();
                StopTimer();
                return retval;
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                StartTimer(IOKind.Write);
                _baseStream.Write(buffer, offset, count);
                StopTimer();
            }
            catch (Exception e)
            {
                HandleException(e);
                throw;
            }
        }

        public override bool CanTimeout => _baseStream.CanTimeout;

        public override int ReadTimeout
        {
            get { return _baseStream.ReadTimeout; }
            set { _baseStream.ReadTimeout = value; }
        }
        public override int WriteTimeout
        {
            get { return _baseStream.WriteTimeout; }
            set { _baseStream.WriteTimeout = value; }
        }

        public override void Close()
        {
            if (IsClosed)
                return;
            IsClosed = true;
            _baseStream.Close();
        }

        public void ResetTimeout(int newTimeout)
        {
            if (newTimeout == System.Threading.Timeout.Infinite || newTimeout == 0)
                _timeout = System.Threading.Timeout.Infinite;
            else
                _timeout = newTimeout;
            _stopwatch.Reset();
        }

        void HandleException(Exception e)
        {
            _stopwatch.Stop();
            ResetTimeout(-1);
        }
    }
}
