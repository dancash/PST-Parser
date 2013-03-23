using System;

namespace PSTParse.NDB
{
    public class BBTENTRY : BTPAGEENTRY
    {
        public BREF BREF;
        public bool Internal;
        public UInt16 BlockByteCount;

        public BBTENTRY(byte[] bytes)
        {
            this.BREF = new BREF(bytes);
            /*this.BREF = new BREF_UNICODE
                            {BID_raw = BitConverter.ToUInt64(bytes, 0), ByteIndex = BitConverter.ToUInt64(bytes, 8)};*/
            this.Internal = this.BREF.IsInternal;
            this.BlockByteCount = BitConverter.ToUInt16(bytes, 16);
        }

        public ulong Key
        {
            get { return BREF.BID; }
        }
    }
}
