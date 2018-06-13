using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class HNID
    {
        public ulong HNID_Type;
        public ulong hnidIndex;
        public ulong hnidBlockIndex;

        public HNID(byte[] bytes)
        {
            var temp = BitConverter.ToUInt64(bytes, 0);
            this.HNID_Type = temp & 0x1F;
            this.hnidIndex = (temp >> 5) & 0x4FF;
            this.hnidBlockIndex = temp >> 16;
        }
    }
}
