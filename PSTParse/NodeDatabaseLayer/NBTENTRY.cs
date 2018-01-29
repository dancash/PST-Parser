using System;

namespace PSTParse.NodeDatabaseLayer
{
    public class NBTENTRY : BTPAGEENTRY
    {
        public ulong NID { get; set; }
        public ulong BID_Data { get; set; }
        public ulong BID_SUB { get; set; }
        public ulong NID_TYPE { get; set; }
        public uint NID_Parent { get; set; }

        public NBTENTRY(byte[] curEntryBytes)
        {
            this.NID = BitConverter.ToUInt64(curEntryBytes, 0);
            this.NID_TYPE = this.NID & 0x1f;
            this.BID_Data = BitConverter.ToUInt64(curEntryBytes,8);
            this.BID_SUB = BitConverter.ToUInt64(curEntryBytes,16);

            this.NID_Parent = BitConverter.ToUInt32(curEntryBytes, 24);
        }
    }
}
