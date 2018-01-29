using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NodeDatabaseLayer;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class HN
    {
        public NBTENTRY HNNode;
        public List<HNBlock> HeapNodes;
        public Dictionary<ulong, NodeDataDTO> HeapSubNode;

        public HN(NodeDataDTO nodeData)
        {
            this.HeapNodes = new List<HNBlock>();
            var numBlocks = nodeData.NodeData.Count;
            for (int i = 0; i < numBlocks; i++)
            {
                var curBlock = new HNBlock(i, nodeData.NodeData[i]);
                this.HeapNodes.Add(curBlock);
            }

            this.HeapSubNode = nodeData.SubNodeData;
        }

        public HNDataDTO GetHIDBytes(HID hid)
        {
            return this.HeapNodes[(int)hid.hidBlockIndex].GetAllocation(hid);
        }
    }
}
