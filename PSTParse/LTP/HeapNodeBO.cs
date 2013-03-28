using PSTParse.NDB;

namespace PSTParse.LTP
{
    public static class HeapNodeBO
    {
        public static HN GetHeapNode(ulong NID)
        {
            return new HN(BlockBO.GetNodeData(NID));
        }

        public static HNDataDTO GetHNHIDBytes(HN heapNode, HID hid)
        {
            var hnblock = heapNode.HeapNodes[(int)hid.hidBlockIndex];
            return hnblock.GetAllocation(hid);
        }
    }
}
