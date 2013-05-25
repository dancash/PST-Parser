using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public class NamedToPropertyLookup
    {
        public static ulong NODE_ID = 0x61;

        public PropertyContext PC;
        public Dictionary<ushort, NAMEID> Lookup; 

        internal byte[] _GUIDs;
        internal byte[] _entries;
        internal byte[] _string;

        

        public NamedToPropertyLookup(PSTFile pst)
        {
            
            this.PC = new PropertyContext(NamedToPropertyLookup.NODE_ID, pst);
            this._GUIDs = this.PC.Properties[0x0002].Data;
            this._entries = this.PC.Properties[0x0003].Data;
            this._string = this.PC.Properties[0x0004].Data;

            this.Lookup = new Dictionary<ushort, NAMEID>();
            for (int i = 0; i < this._entries.Length; i += 8)
            {
                var cur = new NAMEID(this._entries, i, this);
                this.Lookup.Add(cur.PropIndex, cur);
            }
        }
    }
}
