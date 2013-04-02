using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class BTH
    {
        public HN HeapNode;
        public BTHHEADER Header;
        public BTHIndexNode Root;
        public int CurrentLevel;

        public Dictionary<byte[], BTHDataEntry> Properties;

        public BTH(HN heapNode, HID userRoot = null)
        {
            this.HeapNode = heapNode;

            var bthHeaderHID = userRoot ?? heapNode.HeapNodes[0].Header.UserRoot;
            this.Header = new BTHHEADER(HeapNodeBO.GetHNHIDBytes(heapNode, bthHeaderHID));
            this.Root = new BTHIndexNode(this.Header.BTreeRoot, this, (int)this.Header.NumLevels);

            this.Properties = new Dictionary<byte[], BTHDataEntry>(new ArrayUtilities.ByteArrayComparer());

            var stack = new Stack<BTHIndexNode>();
            stack.Push(this.Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Data != null)
                    foreach (var entry in cur.Data.DataEntries)
                        this.Properties.Add(entry.Key, entry);

                if (cur.Children != null)
                    foreach (var child in cur.Children)
                        stack.Push(child);

            }
        }

        public HNDataDTO GetHIDBytes(HID hid)
        {
            return this.HeapNode.GetHIDBytes(hid);
        }

        public Dictionary<ushort, ExchangeProperty> GetExchangeProperties()
        {
            var ret = new Dictionary<ushort, ExchangeProperty>();

            var stack = new Stack<BTHIndexNode>();
            stack.Push(this.Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Data != null)
                    foreach (var entry in cur.Data.DataEntries)
                    {
                        var curKey = BitConverter.ToUInt16(entry.Key, 0);
                        int i = 0;
                        if (curKey == 0x70)
                            i++;
                        ret.Add(curKey, new ExchangeProperty(entry, this));
                    }

                if (cur.Children != null)
                    foreach (var child in cur.Children)
                        stack.Push(child);

            }

            return ret;
        }
    }
}
