using System.Collections.Generic;
using PSTParse.MessageLayer;
using PSTParse.NodeDatabaseLayer;
using static PSTParse.Utilities.Utilities;
using static System.Text.Encoding;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class PropertyContext
    {
        private string _messageClassProperty;

        public BTH BTH { get; }
        public Dictionary<MessageProperty, ExchangeProperty> Properties { get; }
        public ulong NID { get; }
        public string MessageClassProperty =>
            Lazy(ref _messageClassProperty, () => Unicode.GetString(Properties[MessageProperty.MessageClass].Data));

        public PropertyContext(ulong nid, PSTFile pst) : this(BlockBO.GetNodeData(nid, pst)) => NID = nid;
        public PropertyContext(NodeDataDTO nodeData)
        {
            var HN = new HN(nodeData);
            BTH = new BTH(HN);
            Properties = BTH.GetExchangeProperties();
        }
    }
}
