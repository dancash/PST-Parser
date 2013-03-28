using System;

namespace PSTParse.NDB
{
    public class NBTENTRY : BTPAGEENTRY
    {
        public ulong NID { get; set; }
        public ulong BID_Data { get; set; }
        public ulong BID_SUB { get; set; }
        public ulong NID_TYPE { get; set; }
        public uint NID_Parent { get; set; }

        public NBTENTRY(byte[] curEntryBytes, int offset)
        {
            this.NID = BitConverter.ToUInt64(curEntryBytes, offset);
            this.NID_TYPE = this.NID & 31;
            this.BID_Data = BitConverter.ToUInt64(curEntryBytes, offset + 8);
            this.BID_SUB = BitConverter.ToUInt64(curEntryBytes, offset + 16);

            int i = 0;
            if (this.BID_SUB != 0)
                i++;

            this.NID_Parent = BitConverter.ToUInt32(curEntryBytes, 24);
        }
    }
}
