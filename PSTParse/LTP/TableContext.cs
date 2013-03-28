using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class TableContext
    {
        public TCINFOHEADER TCHeader;
        public HN HeapNode;
        public NodeDataDTO NodeData;
        public BTH RowIndexBTH;
        public Dictionary<uint, uint> ReverseRowIndex;
        private TCRowMatrix RowMatrix;

        public TableContext(ulong nid)
        {
            this.NodeData = BlockBO.GetNodeData(nid);
            this.HeapNode = new HN(this.NodeData);

            var tcinfoHID = this.HeapNode.HeapNodes[0].Header.UserRoot;
            var tcinfoHIDbytes = this.HeapNode.GetHIDBytes(tcinfoHID);
            this.TCHeader = new TCINFOHEADER(tcinfoHIDbytes.Data);

            this.RowIndexBTH = new BTH(this.HeapNode,this.TCHeader.RowIndexLocation);
            this.ReverseRowIndex = new Dictionary<uint, uint>();
            foreach(var prop in this.RowIndexBTH.Properties)
            {
                this.ReverseRowIndex.Add(BitConverter.ToUInt32(prop.Value.Data, 0),BitConverter.ToUInt32(prop.Key, 0));
            }
            this.RowMatrix = new TCRowMatrix(this);
        }
    }
}
