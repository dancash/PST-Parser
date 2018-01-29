using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NodeDatabaseLayer;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class BTHIndexNode
    {
        public HID HID;
        public int Level;

        public List<BTHIndexEntry> Entries;
        public List<BTHIndexNode> Children;
        public BTHDataNode Data; 

        public BTHIndexNode(HID hid, BTH tree, int level)
        {
            this.Level = level;
            this.HID = hid;
            if (hid.hidBlockIndex == 0 && hid.hidIndex == 0)
                return;
            
            this.Entries = new List<BTHIndexEntry>();

            if (level == 0)
            {
                this.Data = new BTHDataNode(hid, tree);
                /*
                for (int i = 0; i < bytes.Length; i += (int)tree.Header.KeySize + 4)
                    this.Entries.Add(new BTHIndexEntry(bytes, i, tree.Header));
                this.DataChildren = new List<BTHDataNode>();
                foreach(var entry in this.Entries)
                    this.DataChildren.Add(new BTHDataNode(entry.HID, tree));*/
            } else
            {
                var bytes = tree.GetHIDBytes(hid);
                for (int i = 0; i < bytes.Data.Length; i += (int)tree.Header.KeySize + 4)
                    this.Entries.Add(new BTHIndexEntry(bytes.Data, i, tree.Header));
                this.Children = new List<BTHIndexNode>();
                foreach(var entry in this.Entries)
                    this.Children.Add(new BTHIndexNode(entry.HID, tree, level - 1));
            }

        }

        public bool BlankPassword(PSTFile pst)
        {
            if (this.Data != null)
                return this.Data.BlankPassword(pst);

            foreach (var child in Children)
                child.BlankPassword(pst);
                /*if (child.BlankPassword(Data) != null)
                    return child.BlankPassword(Data);*/

            return false;
        }
    }
}
