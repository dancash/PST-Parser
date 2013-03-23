using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NDB
{
    public class SIENTRY
    {
        public ulong NextChildNID;
        public ulong SLBlockBID;

        public SIENTRY(byte[] bytes)
        {
            this.NextChildNID = BitConverter.ToUInt64(bytes, 0);
            this.SLBlockBID = BitConverter.ToUInt64(bytes, 8);
        }
    }
}
