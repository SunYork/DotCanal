namespace DotCanal.Driver.Utils
{
    public class MSC
    {
        public const int MAX_PACKET_LENGTH = (1 << 24);
        public const int HEADER_PACKET_LENGTH_FIELD_LENGTH = 3;
        public const int HEADER_PACKET_LENGTH_FIELD_OFFSET = 0;
        public const int HEADER_PACKET_LENGTH = 4;
        public const int HEADER_PACKET_NUMBER_FIELD_LENGTH = 1;

        public const byte NULL_TERMINATED_STRING_DELIMITER = 0x00;
        public const byte DEFAULT_PROTOCOL_VERSION = 0x0a;

        public const int FIELD_COUNT_FIELD_LENGTH = 1;

        public const int EVENT_TYPE_OFFSET = 4;
        public const int EVENT_LEN_OFFSET = 9;

        public const long DEFAULT_BINLOG_FILE_START_POSITION = 4;
    }
}
