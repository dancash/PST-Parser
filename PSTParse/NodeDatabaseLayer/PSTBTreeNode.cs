using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace PSTParse.PSTBTree
{
    /*
    public class PSTBTreeNode
    {
        

        public bool Internal { get; set; }
        public long Offset { get; set; }
        public BTPage Page { get; set; }
        private MemoryMappedFile _mmf;

        public List<PSTBTreeNode> Children { get; set; }

        

        public PSTBTreeNode(BREF root, MemoryMappedFile pstmmf, bool isNode)
        {
            this.Internal = root.IsInternal;
            this.Offset = (long)root.IB;
            this._mmf = pstmmf;
            this.Children = new List<PSTBTreeNode>();

            if (isNode || this.Internal)
            {
                using (var mmfview = this._mmf.CreateViewAccessor(Offset, 512))
                {
                    var bytes = new byte[512];
                    mmfview.ReadArray(0, bytes, 0, 512);
                    this.Page = new BTPage(bytes, root);
                    foreach (var child in this.Page.Entries)
                    {
                        if (child is BTENTRY)
                        {
                            var cur = child as BTENTRY;
                            this.Children.Add(new PSTBTreeNode(cur.BREF, this._mmf, isNode));
                        }
                        else if (child is BBTENTRY)
                        {
                            var cur = child as BBTENTRY;
                            var dataSize = cur.BlockByteCount;
                            using(var mffview2 = this._mmf.CreateViewAccessor((long)cur.BREF.IB,dataSize+16))
                            {
                                var b = new byte[dataSize + 16];
                                mffview2.ReadArray(0, b, 0, dataSize + 16);
                                var block = BlockFactory.GetBlock(b, dataSize, true, false);
                                if (!(block is DataBlock))
                                    this.Children.Add(new PSTBTreeNode(cur.BREF, this._mmf, isNode));
                            }
                        }
                        else if (child is NBTENTRY)
                        {
                            //var cur = child as NBTENTRY;
                            //this.Children.Add(new PSTBTreeNode(cur.));
                        }
                    }
                }
            }
        }
    }*/
}
