using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.LTP
{
    public class TCOLDESC
    {
        public uint Tag;
        public ushort DataOffset;
        public ushort DataSize;
        public ushort CEBIndex;

        public TCOLDESC(byte[] bytes, int offset = 0)
        {
            this.Tag = BitConverter.ToUInt32(bytes, offset);
            this.DataOffset = BitConverter.ToUInt16(bytes, offset + 4);
            this.DataSize = bytes[offset + 6];
            this.CEBIndex = bytes[offset + 7];
        }
    }
}
