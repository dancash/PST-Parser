using System;

namespace PSTParse.NodeDatabaseLayer
{
    public class XBLOCK : IBLOCK
    {
        public BlockDataDTO Block;
        public uint BlockType;
        public uint HeaderLevel;
        public uint BIDEntryCount;
        public uint TotalBytes;

        public ulong[] BIDEntries;

        public XBLOCK(BlockDataDTO block)
        {
            this.Block = block;
            this.BlockType = block.Data[0];
            this.HeaderLevel = block.Data[1];
            this.BIDEntryCount = BitConverter.ToUInt16(block.Data, 2);
            this.TotalBytes = BitConverter.ToUInt32(block.Data, 4);
            this.BIDEntries = new ulong[BIDEntryCount];
            for (int i = 0; i < BIDEntryCount; i++)
                BIDEntries[i] = BitConverter.ToUInt64(block.Data, 8 + i*8);
        }
    }
}
