using System;
using System.Collections.Generic;
using System.Text;

namespace DotCanal.DbSync.Events
{
    public class LogHeader
    {
        protected int Type { get; set; }
        protected long When { get; set; }
        protected int EventLen { get; set; }
        protected long ServerId { get; set; }
        protected int Flags { get; set; }
        protected int ChecksumAlg { get; set; }
        protected long Crc { get; set; }


    }
}
