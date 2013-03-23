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

        public BREF(byte[] bref)
        {
            this.BID = BitConverter.ToUInt64(bref, 0);
            this.IB = BitConverter.ToUInt64(bref, 8);

        }
    }
}
