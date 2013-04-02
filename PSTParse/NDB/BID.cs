using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NDB
{
    public class BID
    {
        public ulong BlockID;

        public BID(byte[] bytes, int offset=0)
        {
            this.BlockID = BitConverter.ToUInt64(bytes, offset) & 0xfffffffffffffffe;
        }
    }
}
