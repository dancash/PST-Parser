using System;
using MiscParseUtilities;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class HNBlock
    {
        public HNHDR Header;
        public HNPAGEHDR PageHeader;
        public HNBITMAPHDR BitMapPageHeader;

        public HNPAGEMAP PageMap;

        public UInt16 PageMapOffset;

        private BlockDataDTO _bytes;

        public HNBlock(int blockIndex, BlockDataDTO bytes)
        {
            this._bytes = bytes;

            this.PageMapOffset = BitConverter.ToUInt16(this._bytes.Data, 0);
            this.PageMap = new HNPAGEMAP(this._bytes.Data, this.PageMapOffset);
            if (blockIndex == 0)
            {
                this.Header = new HNHDR(this._bytes.Data);
            } else if (blockIndex % 128 == 8)
            {
                this.BitMapPageHeader = new HNBITMAPHDR(ref this._bytes.Data);
            } else
            {
                this.PageHeader = new HNPAGEHDR(ref this._bytes.Data);
            }
        }

        public HNDataDTO GetAllocation(HID hid)
        {
            var begOffset = this.PageMap.AllocationTable[(int) hid.hidIndex - 1];
            var endOffset = this.PageMap.AllocationTable[(int) hid.hidIndex];
            return new HNDataDTO
                       {
                           Data = this._bytes.Data.RangeSubset(begOffset, endOffset - begOffset),
                           BlockOffset = begOffset,
                           Parent = _bytes
                       };
        }

        public int GetOffset()
        {
            if (this.Header != null)
                return 12;
            if (this.PageHeader != null)
                return 2;
            return 66;
        }
    }
}
