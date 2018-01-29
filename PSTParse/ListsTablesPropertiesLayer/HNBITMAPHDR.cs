using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class HNBITMAPHDR
    {
        public uint HNPageMapOffset;
        public byte[] FillLevel;

        public HNBITMAPHDR(ref byte[] bytes)
        {
            this.HNPageMapOffset = BitConverter.ToUInt16(bytes, 0);
            this.FillLevel = bytes.Skip(2).Take(64).ToArray();
        }
    }
}
