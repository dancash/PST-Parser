using PSTParse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class PCBTHRecord
    {
        public UInt16 PropID;
        public UInt16 PropType;
        public ExchangeProperty PropertyValue;

        public PCBTHRecord(byte[] bytes)
        {
            this.PropID = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
            this.PropType = BitConverter.ToUInt16(bytes.Skip(2).Take(2).ToArray(), 0);
            var prop= this.PropertyValue = ExchangeProperty.PropertyLookupByTypeID[PropType];
            if (!prop.MultiValue)
            {
                if (!prop.Variable)
                {
                    if (prop.ByteCount <= 4 && prop.ByteCount != 0)
                    {
                        this.PropertyValue.Data = bytes.RangeSubset(4, (int) prop.ByteCount);
                    }
                    else
                    {
                        
                    }
                }
            }
            //this.HNID = new HNID(bytes.Skip(4).ToArray());
        }
    }
}
