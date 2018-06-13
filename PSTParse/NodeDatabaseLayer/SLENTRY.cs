using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NodeDatabaseLayer
{
    public class SLENTRY
    {
        public ulong SubNodeNID;
        public ulong SubNodeBID;
        public ulong SubSubNodeBID;

        public SLENTRY(byte[] bytes)
        {
            this.SubNodeNID = BitConverter.ToUInt64(bytes, 0);
            this.SubNodeBID = BitConverter.ToUInt64(bytes, 8);
            this.SubSubNodeBID = BitConverter.ToUInt64(bytes, 16);
        }
    }
}
