using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NodeDatabaseLayer;
using PSTParse.Utilities;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class BTHDataEntry
    {
        public byte[] Key;
        public byte[] Data;

        public ulong DataOffset;
        public ulong DataBlockOffset;
        public BTH ParentTree;

        public BTHDataEntry(HNDataDTO data, int offset, BTH tree)
        {
            this.Key = data.Data.RangeSubset(offset, (int) tree.Header.KeySize);
            //this.Key = bytes.Skip(offset).Take((int)tree.Header.KeySize).ToArray();
            var temp = offset + (int) tree.Header.KeySize;
            this.Data = data.Data.RangeSubset(temp, (int)tree.Header.DataSize);
            this.DataOffset = (ulong) offset + tree.Header.KeySize;
            this.ParentTree = tree;
        }
    }
}
