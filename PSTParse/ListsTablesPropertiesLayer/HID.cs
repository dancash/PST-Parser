using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class HID
    {
        public ulong HID_Type;
        //the index in the allocations for the specific heap block.
        public ulong hidIndex;
        //the index in the block array for this heap
        public ulong hidBlockIndex;

        public HID(byte[] bytes, int offset = 0)
        {
            var temp = BitConverter.ToUInt32(bytes, offset);
            this.HID_Type = temp & 0x1F;
            this.hidIndex = (temp >> 5) & 0x7FF;
            this.hidBlockIndex = BitConverter.ToUInt16(bytes, offset + 2);
        }

    }
}
