using System;

namespace DotCanal.Driver.Common
{
    internal class LowResolutionStopwatch
    {
        long _startTime;
        public static readonly long Frequency = 1000;
        public static readonly bool IsHighResolution = false;

        public long ElapsedMilliseconds { get; private set; }

        public LowResolutionStopwatch()
        {
            ElapsedMilliseconds = 0;
        }

        public void Start()
        {
            _startTime = Environment.TickCount;
        }

        public void Stop()
        {
            long now = Environment.TickCount;
            long elapsed = (now < _startTime) ? Int32.MaxValue - _startTime + now : now - _startTime;
            ElapsedMilliseconds += elapsed;
        }

        public void Reset()
        {
            ElapsedMilliseconds = 0;
            _startTime = 0;
        }

        public TimeSpan Elapsed => new TimeSpan(0, 0, 0, 0, (int)ElapsedMilliseconds);

        public static LowResolutionStopwatch StartNew()
        {
            LowResolutionStopwatch sw = new LowResolutionStopwatch();
            sw.Start();
            return sw;
        }

        public static long GetTimestamp()
        {
            return Environment.TickCount;
        }

        bool IsRunning()
        {
            return (_startTime != 0);
        }
    }
}
