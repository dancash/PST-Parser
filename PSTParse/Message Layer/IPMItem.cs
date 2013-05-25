using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class IPMItem
    {
        private uint _nid;
        public String MessageClass;
        public PropertyContext PC;

        public IPMItem(PSTFile pst, uint nid)
        {
            this._nid = nid;
            this.PC = new PropertyContext(nid, pst);
            this.MessageClass = Encoding.Unicode.GetString(this.PC.Properties[0x1a].Data);

        }

        protected IPMItem()
        {
        }
    }
}
