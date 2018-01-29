using PSTParse.Utilities;
using System;
using System.Collections.Generic;

namespace PSTParse.NDB
{
    public class SIBLOCK : IBLOCK
    {
        public BlockDataDTO DataBlock;
        public UInt16 EntryCount;
        public List<SIENTRY> Entries;
 
        public SIBLOCK(BlockDataDTO dataBlock)
        {
            this.DataBlock = dataBlock;
            var type = dataBlock.Data[0];
            var cLevel = dataBlock.Data[1];
            this.EntryCount = BitConverter.ToUInt16(dataBlock.Data, 2);
            this.Entries = new List<SIENTRY>();
            for(int i =0;i < EntryCount;i++)
                this.Entries.Add(new SIENTRY(dataBlock.Data.RangeSubset(8 + 16*i, 16)));
        }

    }
}
