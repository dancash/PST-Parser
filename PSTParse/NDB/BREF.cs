using System;

namespace PSTParse.NDB
{
    public class BREF
    {
        public UInt64 BID;
        public UInt64 IB;

        public bool IsInternal
        {
            get { return (this.BID & 0x02) > 0; }
        }

        public BREF(byte[] bref, int offset = 0)
        {
            this.BID = BitConverter.ToUInt64(bref, offset);
            this.BID = this.BID & 0xfffffffffffffffe;
            this.IB = BitConverter.ToUInt64(bref, offset + 8);

        }
    }
}
