using System.Collections.Generic;

namespace DotCanal.Driver.Packets.Server
{
    public class RowDataPacket : PacketWithHeaderPacket
    {
        public List<string> Columns = new List<string>();

        public override void FromBytes(MySqlPacket data)
        {
            do
            {
                Columns.Add(data.ReadLenString());
            } while (data.Position < data.Length);
        }

        public override MySqlPacket ToBytes()
        {
            return null;
        }
    }
}
