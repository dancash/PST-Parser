using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.LTP
{
    public class MVPropVarBase
    {
        public UInt32 PropCount;
        private List<ulong> PropOffsets;
        private List<byte[]> PropDataItems; 
        public MVPropVarBase(byte[] bytes)
        {
            this.PropCount = BitConverter.ToUInt32(bytes, 0);
            this.PropOffsets = new List<ulong>();

            for(int i= 0;i < this.PropCount; i++)
                this.PropOffsets.Add(BitConverter.ToUInt64(bytes, 4 + i*8));

            this.PropDataItems = new List<byte[]>();
            for(int i = 0;i < this.PropCount; i++)
            {
                if (i < PropCount-1)
                {
                    this.PropDataItems.Add(
                        bytes.Skip((int) this.PropOffsets[i]).Take((int) (this.PropOffsets[i + 1] - this.PropOffsets[i]))
                            .ToArray());
                }
            }
        }
    }
}
