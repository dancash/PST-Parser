using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscParseUtilities;
using PSTParse.NDB;

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

        public BTHHEADER(BlockDataDTO block)
        {
            var bytes = block.Data;
            this.BType = bytes[0];
            this.KeySize = bytes[1];
            this.DataSize = bytes[2];
            this.BTreeRoot = new HID(bytes.RangeSubset(4, 4));

        }
    }
}
