using System;
using System.Collections.Generic;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public class PropertyContext
    {
        public BTH BTH;

        public Dictionary<UInt16,ExchangeProperty> Properties;

        public PropertyContext(ulong nid)
        {
            var bytes = BlockBO.GetNodeData(nid);
            var HN = new HN(bytes);
            this.BTH = new BTH(HN);
            this.Properties = this.BTH.GetExchangeProperties();
        }
    }
}
