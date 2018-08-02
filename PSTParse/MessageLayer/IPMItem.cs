using System.Text;
using PSTParse.ListsTablesPropertiesLayer;

namespace PSTParse.MessageLayer
{
    public class IPMItem
    {
        private readonly uint _nid;

        public string MessageClass { get; set; }
        public PropertyContext PC { get; set; }

        public IPMItem(PSTFile pst, uint nid)
        {
            _nid = nid;
            PC = new PropertyContext(nid, pst);
            MessageClass = Encoding.Unicode.GetString(PC.Properties[(MessageProperty)0x1a].Data);

        }

        //fail
        protected IPMItem()
        {
        }
    }
}
