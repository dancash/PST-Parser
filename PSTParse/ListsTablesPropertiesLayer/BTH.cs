using PSTParse.MessageLayer;
using PSTParse.Utilities;
using System;
using System.Collections.Generic;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class BTH
    {
        public HN HeapNode { get; set; }
        public BTHHEADER Header { get; set; }
        public BTHIndexNode Root { get; set; }
        public int CurrentLevel { get; set; }
        public Dictionary<byte[], BTHDataEntry> Properties { get; set; }

        public BTH(HN heapNode, HID userRoot = null)
        {
            HeapNode = heapNode;

            var bthHeaderHID = userRoot ?? heapNode.HeapNodes[0].Header.UserRoot;
            Header = new BTHHEADER(HeapNodeBO.GetHNHIDBytes(heapNode, bthHeaderHID));
            Root = new BTHIndexNode(Header.BTreeRoot, this, (int)Header.NumLevels);

            Properties = new Dictionary<byte[], BTHDataEntry>(new ArrayUtilities.ByteArrayComparer());

            var stack = new Stack<BTHIndexNode>();
            stack.Push(Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                try
                {
                    if (cur.Data != null)
                        foreach (var entry in cur.Data.DataEntries)
                            Properties.Add(entry.Key, entry);
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot display this view.  Failed to create all properties in this location", ex);
                }


                if (cur.Children != null)
                    foreach (var child in cur.Children)
                        stack.Push(child);

            }
        }

        public HNDataDTO GetHIDBytes(HID hid)
        {
            return HeapNode.GetHIDBytes(hid);
        }

        public Dictionary<MessageProperty, ExchangeProperty> GetExchangeProperties()
        {
            var ret = new Dictionary<MessageProperty, ExchangeProperty>();

            var stack = new Stack<BTHIndexNode>();
            stack.Push(Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Data != null)
                    foreach (var entry in cur.Data.DataEntries)
                    {
                        var curKey = BitConverter.ToUInt16(entry.Key, 0);
                        int i = 0;
                        if (curKey == 0x02)
                            i++;
                        ret.Add((MessageProperty)curKey, new ExchangeProperty(entry, this));
                    }

                if (cur.Children != null)
                    foreach (var child in cur.Children)
                        stack.Push(child);

            }

            return ret;
        }
    }
}
