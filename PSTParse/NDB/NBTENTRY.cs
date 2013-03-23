using System;

namespace PSTParse.NDB
{
    public class NBTENTRY : BTPAGEENTRY
    {
        public ulong NID { get; set; }
        public ulong BID_Data { get; set; }
        public ulong BID_SUB { get; set; }
        public ulong NID_TYPE { get; set; }
        public ulong NID_Parent { get; set; }

        public NBTENTRY(byte[] curEntryBytes)
        {
            this.NID = BitConverter.ToUInt64(curEntryBytes, 0);
            this.NID_TYPE = this.NID >> 59;
            this.BID_Data = (BitConverter.ToUInt64(curEntryBytes, 8) << 2) >> 2;
            this.BID_SUB = (BitConverter.ToUInt64(curEntryBytes, 16) << 2) >> 2;
            this.NID_Parent = (BitConverter.ToUInt64(curEntryBytes, 24) << 2) >> 2;
        }
    }
}
