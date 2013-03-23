using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.LTP
{
    public class PCBTHRecord
    {
        public UInt16 PropID;
        public UInt16 PropType;
        public HNID HNID;

        public PCBTHRecord(byte[] bytes)
        {
            this.PropID = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
            this.PropType = BitConverter.ToUInt16(bytes.Skip(2).Take(2).ToArray(), 0);
            this.HNID = new HNID(bytes.Skip(4).ToArray());
        }
    }
}
