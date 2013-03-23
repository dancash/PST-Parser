using System;
using System.Linq;

namespace PSTParse.NDB
{
    public class BTENTRY : BTPAGEENTRY
    {
        private ulong _btkey;
        public BREF BREF;

        public BTENTRY(byte[] bytes)
        {
            this._btkey = BitConverter.ToUInt64(bytes, 0);
            this.BREF = new BREF(bytes.Skip(8).Take(16).ToArray());
            /*this.BREF = new BREF_UNICODE
                            {BID_raw = BitConverter.ToUInt64(bytes, 8), ByteIndex = BitConverter.ToUInt64(bytes, 16)};*/
        }

        public ulong Key
        {
            get { return this._btkey; }
        }
    }
}
