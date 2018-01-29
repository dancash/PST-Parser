using PSTParse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.LTP
{
    public class BTHIndexEntry
    {
        public byte[] Key;
        public HID HID;

        public BTHIndexEntry(byte[] bytes, int offset, BTHHEADER header)
        {
            this.Key = bytes.RangeSubset(offset,(int)header.KeySize);
            var temp = offset + (int) header.KeySize;
            this.HID = new HID(bytes.RangeSubset(temp, 4));

        }
    }
}
