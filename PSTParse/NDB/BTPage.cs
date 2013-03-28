using System;
using System.Collections.Generic;
using System.Linq;

namespace PSTParse.NDB
{
    public class BTPage
    {
        private PageTrailer _trailer;
        private int _numEntries;
        private int _maxEntries;
        private int _cbEnt;
        private int _cLevel;
        private bool _isNBT;
        private BREF _ref;

        public List<BTPAGEENTRY> Entries;

        public List<BTPage> InternalChildren;

        public bool IsNode { get { return this._trailer.PageType == PageType.NBT; } }
        public bool IsBlock { get { return this._trailer.PageType == PageType.BBT; } }

        public ulong BID { get { return this._trailer.BID; } }

        public BTPage(byte[] pageData, BREF _ref)
        {
            this._ref = _ref;
            this.InternalChildren = new List<BTPage>();
            this._trailer = new PageTrailer(pageData.Skip(496).Take(16).ToArray());
            this._numEntries = pageData[488];
            this._maxEntries = pageData[489];
            this._cbEnt = pageData[490];
            this._cLevel = pageData[491];

            this.Entries = new List<BTPAGEENTRY>();
            for (var i = 0; i < this._numEntries; i++)
            {
                var offset = i*this._cbEnt;
                var curEntryBytes = pageData.Skip(i*this._cbEnt).Take(this._cbEnt).ToArray();
                if (this._cLevel == 0)
                {
                    if (this._trailer.PageType == PageType.NBT)
                        this.Entries.Add(new NBTENTRY(pageData, offset));
                    else
                    {
                        var curEntry = new BBTENTRY(curEntryBytes);
                        this.Entries.Add(curEntry);
                    }
                }
                else
                {
                    //btentries
                    var entry = new BTENTRY(curEntryBytes);
                    this.Entries.Add(entry);
                    using (var view = PSTFile.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        this.InternalChildren.Add(new BTPage(bytes, entry.BREF));
                    }
                }
            }
        }

        public BBTENTRY GetBIDBBTEntry(ulong BID)
        {

            for (int i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];
                if (i == this.Entries.Count-1)
                {

                    if (entry is BTENTRY)
                        return this.InternalChildren[i].GetBIDBBTEntry(BID);
                    else
                    {
                        var temp = entry as BBTENTRY;
                        return temp;
                    }

                }
                var entry2 = this.Entries[i + 1];
                if (entry is BTENTRY)
                {
                    var cur = entry as BTENTRY;
                    var next = entry2 as BTENTRY;
                    if (BID >= cur.Key && BID < next.Key)
                        return this.InternalChildren[i].GetBIDBBTEntry(BID);
                }
                else if (entry is BBTENTRY)
                {
                    var cur = entry as BBTENTRY;
                    if (BID == cur.Key)
                        return cur;
                }
            }
            return null;
        }

        public Tuple<ulong,ulong> GetNIDBID(ulong NID)
        {
            var isBTEntry = this.Entries[0] is BTENTRY;
            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (i == this.Entries.Count - 1)
                {
                    if (isBTEntry)
                        return this.InternalChildren[i].GetNIDBID(NID);
                    var cur = this.Entries[i] as NBTENTRY;
                    return new Tuple<ulong, ulong>(cur.BID_Data,cur.BID_SUB);
                }

                var curEntry = this.Entries[i];
                var nextEntry = this.Entries[i + 1];
                if (isBTEntry)
                {
                    var cur = curEntry as BTENTRY;
                    var next = nextEntry as BTENTRY;
                    if (NID >= cur.Key && NID < next.Key)
                        return this.InternalChildren[i].GetNIDBID(NID);
                }
                else
                {
                    var cur = curEntry as NBTENTRY;
                    if (NID == cur.NID)
                        return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                }
            }
            return new Tuple<ulong, ulong>(0, 0);
        }
    }
}
