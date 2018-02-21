using PSTParse.Utilities;
using System;
using System.IO;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class EntryID
    {
        public uint Flags { get; set; }
        public byte[] PSTUID { get; set; }
        public ulong NID { get; set; }

        public EntryID(byte[] bytes, int offset = 0)
        {
            if (bytes.Length == 0)
            {
                throw new InvalidDataException("The entry id was invalid, try running a PST repair");
            }

            Flags = BitConverter.ToUInt32(bytes, offset);
            PSTUID = bytes.RangeSubset(4+offset, 16);
            NID = BitConverter.ToUInt32(bytes, offset + 20);
        }
    }
}