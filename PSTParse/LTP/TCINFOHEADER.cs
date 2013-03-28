using System;
using System.Collections.Generic;

namespace PSTParse.LTP
{
    public class TCINFOHEADER
    {
        public byte Type;
        public ushort ColumnCount;
        public ushort EndOffset48;
        public ushort EndOffset2;
        public ushort EndOffset1;
        public ushort EndOffsetCEB;
        public HID RowIndexLocation;
        public ulong RowMatrixLocation;

        public List<TCOLDESC> ColumnsDescriptors; 

        public TCINFOHEADER(byte[] bytes)
        {
            this.Type = bytes[0];
            this.ColumnCount = bytes[1];
            this.EndOffset48 = BitConverter.ToUInt16(bytes, 2);
            this.EndOffset2 = BitConverter.ToUInt16(bytes, 4);
            this.EndOffset1 = BitConverter.ToUInt16(bytes, 6);
            this.EndOffsetCEB = BitConverter.ToUInt16(bytes, 8);
            this.RowIndexLocation = new HID(bytes, 10);
            this.RowMatrixLocation = BitConverter.ToUInt64(bytes, 14);

            this.ColumnsDescriptors = new List<TCOLDESC>();
            for(var i = 0;i < this.ColumnCount; i++)
            {
                this.ColumnsDescriptors.Add(new TCOLDESC(bytes, 22 + i*8));
            }
        }
    }
}
