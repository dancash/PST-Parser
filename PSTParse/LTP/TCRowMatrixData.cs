using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class TCRowMatrixData
    {
        public Dictionary<uint, byte[]> ColumnXREF;

        public TCRowMatrixData(byte[] bytes, TableContext context, int offset = 0)
        {
            this.ColumnXREF = new Dictionary<uint, byte[]>();
            
            //var rowSize = context.TCHeader.EndOffsetCEB;
            foreach (var col in context.TCHeader.ColumnsDescriptors)
                this.ColumnXREF.Add(col.Tag, bytes.RangeSubset(offset + col.DataOffset, col.DataSize));
        }
    }
}
