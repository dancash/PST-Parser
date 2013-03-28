using System;
using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class EntryID
    {
        public uint Flags;
        public byte[] PSTUID;
        public ulong NID;

        public EntryID(byte[] bytes, int offset = 0)
        {
            this.Flags = BitConverter.ToUInt32(bytes, offset);
            this.PSTUID = bytes.RangeSubset(2, 16 + offset);
            this.NID = BitConverter.ToUInt32(bytes, offset + 18);
        }
    }
}
