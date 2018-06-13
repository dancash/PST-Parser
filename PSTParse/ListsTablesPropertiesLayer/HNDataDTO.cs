using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NodeDatabaseLayer;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class HNDataDTO
    {
        public BlockDataDTO Parent;
        public long BlockOffset;
        public byte[] Data;
    }
}
