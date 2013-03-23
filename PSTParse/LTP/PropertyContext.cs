using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public class PropertyContext
    {
        public BTH BTH;

        public PropertyContext(ulong nid)
        {
            var bytes = BlockBO.GetNodeData(nid);
            var HN = new HN(bytes);
            this.BTH = new BTH(HN);
        }
    }
}
