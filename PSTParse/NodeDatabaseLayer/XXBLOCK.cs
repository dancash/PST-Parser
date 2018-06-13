using System;

namespace PSTParse.NodeDatabaseLayer
{
    public class XXBLOCK : IBLOCK
    {
        public byte Type;
        public byte CLevel;
        public UInt16 TotalChildren;
        public uint TotalBytes;
        public BlockDataDTO Block;

        public ulong[] XBlockBIDs;

        public XXBLOCK(BlockDataDTO block)
        {
            this.Block = block;

            this.Type = block.Data[0];
            this.CLevel = block.Data[1];
            this.TotalChildren = BitConverter.ToUInt16(block.Data, 2);
            this.TotalBytes = BitConverter.ToUInt32(block.Data, 4);
            this.XBlockBIDs = new ulong[this.TotalChildren];
            for (var i = 0; i < TotalChildren; i++)
                this.XBlockBIDs[i] = BitConverter.ToUInt64(block.Data, 8 + 8*i);

        }
    }
}
