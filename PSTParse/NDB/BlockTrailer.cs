using System;

namespace PSTParse.NDB
{
    public class BlockTrailer
    {
        public uint DataSize { get; set; }
        public uint WSig { get; set; }
        public uint CRC { get; set; }
        public ulong BID_raw { get; set; }

        public BlockTrailer(byte[] bytes, int offset)
        {
            this.DataSize = BitConverter.ToUInt16(bytes, offset);
            this.WSig = BitConverter.ToUInt16(bytes, 2 + offset);
            this.CRC = BitConverter.ToUInt32(bytes, 4 + offset);
            this.BID_raw = BitConverter.ToUInt64(bytes, 8 + offset);
        }
    }
}
