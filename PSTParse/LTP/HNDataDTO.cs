using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class HNDataDTO
    {
        public BlockDataDTO Parent;
        public long BlockOffset;
        public byte[] Data;
    }
}
