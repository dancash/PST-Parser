using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class BTH
    {
        private HN _heapNode;
        public BTHHEADER Header;
        public BTHIndexNode Root;
        //public BTHIndexAllocationRecords IndexRecords;
        public int CurrentLevel;

        public BTH(HN heapNode)
        {
            this._heapNode = heapNode;

            var bthHeaderHID = heapNode.HeapNodes[0].Header.UserRoot;
            this.Header = new BTHHEADER(HeapNodeBO.GetHNHIDBytes(heapNode, bthHeaderHID));
            this.Root = new BTHIndexNode(this.Header.BTreeRoot, this, (int)this.Header.NumLevels);

        }

        public BlockDataDTO GetHIDBytes(HID hid)
        {
            return this._heapNode.HeapNodes[(int) hid.hidBlockIndex].GetAllocation(hid);

        }
    }
}
