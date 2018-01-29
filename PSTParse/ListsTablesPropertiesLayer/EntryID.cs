using PSTParse.Utilities;
using System;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class EntryID
    {
        public uint Flags;
        public byte[] PSTUID;
        public ulong NID;

        public EntryID(byte[] bytes, int offset = 0)
        {
            this.Flags = BitConverter.ToUInt32(bytes, offset);
            this.PSTUID = bytes.RangeSubset(4+offset, 16);
            this.NID = BitConverter.ToUInt32(bytes, offset + 20);
        }
    }
}
