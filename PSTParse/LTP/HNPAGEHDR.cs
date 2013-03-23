using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.LTP
{
    public class HNPAGEHDR
    {
        public UInt16 HNPageMapOffset;
        public HNPAGEHDR(ref byte[] bytes)
        {
            this.HNPageMapOffset = BitConverter.ToUInt16(bytes, 0);
        }
    }
}
