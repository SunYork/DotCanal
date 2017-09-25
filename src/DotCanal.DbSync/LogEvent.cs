using System;
using System.Collections.Generic;
using System.Text;

namespace DotCanal.DbSync
{
    public abstract class LogEvent
    {
        public const int BINLOG_VERSION = 4;
        public const string SERVER_VERSION = "5.0";

        public const int EVENT_TYPE_OFFSET = 4;
        public const int SERVER_ID_OFFSET = 5;
    }
}
