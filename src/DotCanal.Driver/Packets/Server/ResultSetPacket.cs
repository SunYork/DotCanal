using System.Collections.Generic;
using System.Net;

namespace DotCanal.Driver.Packets.Server
{
    public class ResultSetPacket
    {
        public EndPoint SourceAddress { get; set; }
        public List<FieldPacket> FieldDescriptors = new List<FieldPacket>();
        public List<string> FieldValues = new List<string>();
    }
}
