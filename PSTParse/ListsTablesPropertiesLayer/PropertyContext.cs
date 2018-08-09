using System;
using System.Collections.Generic;
using System.Text;
using PSTParse.MessageLayer;
using PSTParse.NodeDatabaseLayer;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class PropertyContext
    {
        private Lazy<string> _messageClassProperty;
        public BTH BTH { get; }
        public Dictionary<MessageProperty, ExchangeProperty> Properties { get; } = new Dictionary<MessageProperty, ExchangeProperty>();
        public ulong NID { get; }
        public string MessageClassProperty => _messageClassProperty.Value;

        public PropertyContext(ulong nid, PSTFile pst) : this(BlockBO.GetNodeData(nid, pst))
        {
            NID = nid;
        }

        public PropertyContext(NodeDataDTO nodeData)
        {
            var HN = new HN(nodeData);
            BTH = new BTH(HN);
            Properties = BTH.GetExchangeProperties();
            _messageClassProperty = new Lazy<string>(GetMessageClass);
        }

        private string GetMessageClass()
        {
            return Encoding.Unicode.GetString(Properties[(MessageProperty)0x1a].Data);
        }
    }
}
