using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NodeDatabaseLayer
{
    public class BlockDataDTO
    {
        public BlockDataDTO Parent;
        public byte[] Data;
        public ulong PstOffset;
        public uint CRC32;
        public uint CRCOffset;
        public BBTENTRY BBTEntry;
    }
}
