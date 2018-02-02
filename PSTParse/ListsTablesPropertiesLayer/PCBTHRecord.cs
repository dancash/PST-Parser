using PSTParse.Utilities;
using System;
using System.Linq;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class PCBTHRecord
    {
        public UInt16 PropID;
        public UInt16 PropType;
        public ExchangeProperty PropertyValue;

        public PCBTHRecord(byte[] bytes)
        {
            PropID = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
            PropType = BitConverter.ToUInt16(bytes.Skip(2).Take(2).ToArray(), 0);
            var prop= PropertyValue = ExchangeProperty.PropertyLookupByTypeID[(ExchangeProperty.PropType)PropType];
            if (!prop.MultiValue)
            {
                if (!prop.Variable)
                {
                    if (prop.ByteCount <= 4 && prop.ByteCount != 0)
                    {
                        PropertyValue.Data = bytes.RangeSubset(4, (int) prop.ByteCount);
                    }
                    else
                    {
                        
                    }
                }
            }
            //HNID = new HNID(bytes.Skip(4).ToArray());
        }
    }
}