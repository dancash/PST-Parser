using PSTParse.MessageLayer;
using PSTParse.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class TCRowMatrixData : IEnumerable<ExchangeProperty>
    {
        private readonly BTH _heap;

        public Dictionary<MessageProperty, byte[]> ColumnXREF { get; set; }

        public TCRowMatrixData(byte[] bytes, TableContext context, BTH heap, int offset = 0)
        {
            ColumnXREF = new Dictionary<MessageProperty, byte[]>();
            _heap = heap;

            //todo: cell existence test
            //var rowSize = context.TCHeader.EndOffsetCEB;
            foreach (var col in context.TCHeader.ColumnsDescriptors)
            {
                ColumnXREF.Add((MessageProperty)col.Tag, bytes.RangeSubset(offset + col.DataOffset, col.DataSize));
            }
        }

        public IEnumerator<ExchangeProperty> GetEnumerator()
        {
            foreach (var col in this.ColumnXREF)
            {
                var uIntKey = (UInt16)(((uint)col.Key) >> 16);
                var type = (UInt16)((uint)col.Key & 0xFFFF);
                yield return new ExchangeProperty(uIntKey, type, _heap, col.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
