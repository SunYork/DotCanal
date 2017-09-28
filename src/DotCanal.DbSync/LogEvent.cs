using DotCanal.Common.Log;
using Microsoft.Extensions.Logging;
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
        public const int EVENT_LEN_OFFSET = 9;
        public const int LOG_POS_OFFSET = 13;
        public const int FLAGS_OFFSET = 17;

        public const int QUERY_HEADER_MINIMAL_LEN = 11;
        public const int QUERY_HEADER_LEN = QUERY_HEADER_MINIMAL_LEN + 2;

        public const int UNKNOWN_EVENT = 0;
        public const int START_EVENT_V3 = 1;
        public const int QUERY_EVENT = 2;
        public const int STOP_EVENT = 3;
        public const int ROTATE_EVENT = 4;
        public const int INTVAR_EVENT = 5;
        public const int LOAD_EVENT = 6;
        public const int SLAVE_EVENT = 7;
        public const int CREATE_FILE_EVENT = 8;
        public const int APPEND_BLOCK_EVENT = 9;
        public const int EXEC_LOAD_EVENT = 10;
        public const int DELETE_FILE_EVENT = 11;
        public const int NEW_LOAD_EVENT = 12;
        public const int RAND_EVENT = 13;
        public const int USER_VAR_EVENT = 14;
        public const int FORMAT_DESCRIPTION_EVENT = 15;
        public const int XID_EVENT = 16;
        public const int BEGIN_LOAD_QUERY_EVENT = 17;
        public const int EXECUTE_LOAD_QUERY_EVENT = 18;
        public const int TABLE_MAP_EVENT = 19;
        public const int PRE_GA_WRITE_ROWS_EVENT = 20;
        public const int PRE_GA_UPDATE_ROWS_EVENT = 21;
        public const int PRE_GA_DELETE_ROWS_EVENT = 22;
        public const int WRITE_ROWS_EVENT_V1 = 23;
        public const int UPDATE_ROWS_EVENT_V1 = 24;
        public const int DELETE_ROWS_EVENT_V1 = 25;
        public const int INCIDENT_EVENT = 26;
        public const int HEARTBEAT_LOG_EVENT = 27;
        public const int IGNORABLE_LOG_EVENT = 28;
        public const int ROWS_QUERY_LOG_EVENT = 29;
        public const int WRITE_ROWS_EVENT = 30;
        public const int UPDATE_ROWS_EVENT = 31;
        public const int DELETE_ROWS_EVENT = 32;
        public const int GTID_LOG_EVENT = 33;
        public const int ANONYMOUS_GTID_LOG_EVENT = 34;
        public const int PREVIOUS_GTIDS_LOG_EVENT = 35;
        public const int MYSQL_EVENTS_END = 36;
        public const int MARIA_EVENTS_BEGIN = 160;
        public const int ANNOTATE_ROWS_EVENT = 160;
        public const int BINLOG_CHECKPOINT_EVENT = 161;
        public const int GTID_EVENT = 162;
        public const int GTID_LIST_EVENT = 163;
        public const int ENUM_END_EVENT = 164;
        public const int EXTRA_ROW_INFO_LEN_OFFSET = 0;
        public const int EXTRA_ROW_INFO_FORMAT_OFFSET = 1;
        public const int EXTRA_ROW_INFO_HDR_BYTES = 2;
        public const int EXTRA_ROW_INFO_MAX_PAYLOAD = (255 - EXTRA_ROW_INFO_HDR_BYTES);
        public const int BINLOG_CHECKSUM_ALG_OFF = 0;
        public const int BINLOG_CHECKSUM_ALG_CRC32 = 1;
        public const int BINLOG_CHECKSUM_ALG_ENUM_END = 2;
        public const int BINLOG_CHECKSUM_ALG_UNDEF = 255;
        public const int CHECKSUM_CRC32_SIGNATURE_LEN = 4;
        public const int BINLOG_CHECKSUM_ALG_DESC_LEN = 1;
        public const int BINLOG_CHECKSUM_LEN = CHECKSUM_CRC32_SIGNATURE_LEN;

        public const int MARIA_SLAVE_CAPABILITY_UNKNOWN = 0;
        public const int MARIA_SLAVE_CAPABILITY_ANNOTATE = 1;
        public const int MARIA_SLAVE_CAPABILITY_TOLERATE_HOLES = 2;
        public const int MARIA_SLAVE_CAPABILITY_BINLOG_CHECKPOINT = 3;
        public const int MARIA_SLAVE_CAPABILITY_GTID = 4;
        public const int MARIA_SLAVE_CAPABILITY_MINE = MARIA_SLAVE_CAPABILITY_GTID;

        public const int LOG_EVENT_IGNORABLE_F = 0x80;

        public const int MYSQL_TYPE_DECIMAL = 0;
        public const int MYSQL_TYPE_TINY = 1;
        public const int MYSQL_TYPE_SHORT = 2;
        public const int MYSQL_TYPE_LONG = 3;
        public const int MYSQL_TYPE_FLOAT = 4;
        public const int MYSQL_TYPE_DOUBLE = 5;
        public const int MYSQL_TYPE_NULL = 6;
        public const int MYSQL_TYPE_TIMESTAMP = 7;
        public const int MYSQL_TYPE_LONGLONG = 8;
        public const int MYSQL_TYPE_INT24 = 9;
        public const int MYSQL_TYPE_DATE = 10;
        public const int MYSQL_TYPE_TIME = 11;
        public const int MYSQL_TYPE_DATETIME = 12;
        public const int MYSQL_TYPE_YEAR = 13;
        public const int MYSQL_TYPE_NEWDATE = 14;
        public const int MYSQL_TYPE_VARCHAR = 15;
        public const int MYSQL_TYPE_BIT = 16;
        public const int MYSQL_TYPE_TIMESTAMP2 = 17;
        public const int MYSQL_TYPE_DATETIME2 = 18;
        public const int MYSQL_TYPE_TIME2 = 19;
        public const int MYSQL_TYPE_JSON = 245;
        public const int MYSQL_TYPE_NEWDECIMAL = 246;
        public const int MYSQL_TYPE_ENUM = 247;
        public const int MYSQL_TYPE_SET = 248;
        public const int MYSQL_TYPE_TINY_BLOB = 249;
        public const int MYSQL_TYPE_MEDIUM_BLOB = 250;
        public const int MYSQL_TYPE_LONG_BLOB = 251;
        public const int MYSQL_TYPE_BLOB = 252;
        public const int MYSQL_TYPE_VAR_STRING = 253;
        public const int MYSQL_TYPE_STRING = 254;
        public const int MYSQL_TYPE_GEOMETRY = 255;

        public String GetTypeName(int type)
        {
            switch (type)
            {
                case START_EVENT_V3:
                    return "Start_v3";
                case STOP_EVENT:
                    return "Stop";
                case QUERY_EVENT:
                    return "Query";
                case ROTATE_EVENT:
                    return "Rotate";
                case INTVAR_EVENT:
                    return "Intvar";
                case LOAD_EVENT:
                    return "Load";
                case NEW_LOAD_EVENT:
                    return "New_load";
                case SLAVE_EVENT:
                    return "Slave";
                case CREATE_FILE_EVENT:
                    return "Create_file";
                case APPEND_BLOCK_EVENT:
                    return "Append_block";
                case DELETE_FILE_EVENT:
                    return "Delete_file";
                case EXEC_LOAD_EVENT:
                    return "Exec_load";
                case RAND_EVENT:
                    return "RAND";
                case XID_EVENT:
                    return "Xid";
                case USER_VAR_EVENT:
                    return "User var";
                case FORMAT_DESCRIPTION_EVENT:
                    return "Format_desc";
                case TABLE_MAP_EVENT:
                    return "Table_map";
                case PRE_GA_WRITE_ROWS_EVENT:
                    return "Write_rows_event_old";
                case PRE_GA_UPDATE_ROWS_EVENT:
                    return "Update_rows_event_old";
                case PRE_GA_DELETE_ROWS_EVENT:
                    return "Delete_rows_event_old";
                case WRITE_ROWS_EVENT_V1:
                    return "Write_rows_v1";
                case UPDATE_ROWS_EVENT_V1:
                    return "Update_rows_v1";
                case DELETE_ROWS_EVENT_V1:
                    return "Delete_rows_v1";
                case BEGIN_LOAD_QUERY_EVENT:
                    return "Begin_load_query";
                case EXECUTE_LOAD_QUERY_EVENT:
                    return "Execute_load_query";
                case INCIDENT_EVENT:
                    return "Incident";
                case HEARTBEAT_LOG_EVENT:
                    return "Heartbeat";
                case IGNORABLE_LOG_EVENT:
                    return "Ignorable";
                case ROWS_QUERY_LOG_EVENT:
                    return "Rows_query";
                case WRITE_ROWS_EVENT:
                    return "Write_rows";
                case UPDATE_ROWS_EVENT:
                    return "Update_rows";
                case DELETE_ROWS_EVENT:
                    return "Delete_rows";
                case GTID_LOG_EVENT:
                    return "Gtid";
                case ANONYMOUS_GTID_LOG_EVENT:
                    return "Anonymous_Gtid";
                case PREVIOUS_GTIDS_LOG_EVENT:
                    return "Previous_gtids";
                default:
                    return "Unknown";
            }
        }

        protected ILogger logger = ApplicationLogging.CreateLogger<LogEvent>();


    }
}
