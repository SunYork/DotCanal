using System;
using System.Data.Common;

namespace DotCanal.Common.Exceptions
{
    public class DotCanalException : DbException
    {
        public DotCanalException() { }

        public DotCanalException(string msg)
            : base(msg) { }

        public DotCanalException(string msg, Exception ex)
            : base(msg, ex) { }

        public DotCanalException(string msg, bool isFatal, Exception inner)
            : base(msg, inner)
        {
            IsFatal = isFatal;
        }

        public DotCanalException(string msg, int errno, Exception inner)
            : this(msg, inner)
        {
            Number = errno;
            Data.Add("Server Error Code", errno);
        }

        public DotCanalException(string msg, int errno)
            : this(msg, errno, null) { }

        public DotCanalException(UInt32 code, string sqlState, string msg) : base(msg)
        {
            Code = code;
            SqlState = sqlState;
        }

        public int Number { get; }

        internal bool IsFatal { get; }

        public string SqlState { get; private set; }

        public UInt32 Code { get; private set; }
    }
}
