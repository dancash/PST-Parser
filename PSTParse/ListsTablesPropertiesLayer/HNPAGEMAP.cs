using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class HNPAGEMAP
    {
        public uint AllocationsCount;
        public uint FreeItemsCount;
        public List<UInt16> AllocationTable;

        public HNPAGEMAP(byte[] bytes, int offset)
        {
            this.AllocationsCount = BitConverter.ToUInt16(bytes, offset);
            this.FreeItemsCount = BitConverter.ToUInt16(bytes, offset+2);
            this.AllocationTable = new List<UInt16>();

            for(int i= 0;i < AllocationsCount+1;i++)
                this.AllocationTable.Add(BitConverter.ToUInt16(bytes,offset+4+i*2));
        }
    }
}
