using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;
using PSTParse.Utilities;

namespace PSTParse.LTP
{
    public class BTHHEADER
    {
        public uint BType;
        //must be 2,4,8,16
        public uint KeySize;
        //must be >0 <=32
        public uint DataSize;
        public uint NumLevels;
        public HID BTreeRoot;

        public BTHHEADER(HNDataDTO block)
        {
            var bytes = block.Data;
            this.BType = bytes[0];
            this.KeySize = bytes[1];
            this.DataSize = bytes[2];
            this.NumLevels = bytes[3];
            this.BTreeRoot = new HID(bytes.RangeSubset(4, 4));

        }
    }
}
