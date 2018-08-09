using PSTParse.ListsTablesPropertiesLayer;

namespace PSTParse.MessageLayer
{
    public class IPMItem
    {
        public string MessageClass => PropertyContext.MessageClassProperty;
        protected PropertyContext PropertyContext { get; }

        //public IPMItem(PSTFile pst, ulong nid)
        //{
        //    PropertyContext = new PropertyContext(nid, pst);
        //    MessageClass = Encoding.Unicode.GetString(PropertyContext.Properties[(MessageProperty)0x1a].Data);
        //}

        public IPMItem(PSTFile pst, PropertyContext propertyContext)
        {
            PropertyContext = propertyContext;
            //MessageClass = Encoding.Unicode.GetString(PropertyContext.Properties[(MessageProperty)0x1a].Data);
        }
    }
}
