using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscParseUtilities;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class ExchangeProperty
    {
        public static Dictionary<UInt16, ExchangeProperty>
            PropertyLookupByTypeID = new Dictionary
                <UInt16, ExchangeProperty>
                                         {
                                             {0x0002,new ExchangeProperty{ByteCount = 2,Type = 0x0002,MultiValue = false, Variable = false}},
                                             {0x0003,new ExchangeProperty{ByteCount = 4,Type = 0x0003,MultiValue = false, Variable = false}},
                                             {0x0004,new ExchangeProperty{ByteCount = 4,Type = 0x0004,MultiValue = false, Variable = false}},
                                             {0x0005,new ExchangeProperty{ByteCount = 8,Type = 0x0005,MultiValue = false, Variable = false}},
                                             {0x0006,new ExchangeProperty{ByteCount = 8,Type = 0x0006,MultiValue = false, Variable = false}},
                                             {0x0007,new ExchangeProperty{ByteCount = 8,Type = 0x0007,MultiValue = false, Variable = false}},
                                             {0x000A,new ExchangeProperty{ByteCount = 4,Type = 0x000A,MultiValue = false, Variable = false}},
                                             {0x000B,new ExchangeProperty{ByteCount = 1,Type = 0x000B,MultiValue = false, Variable = false}},
                                             {0x0014,new ExchangeProperty{ByteCount = 8,Type = 0x0014,MultiValue = false, Variable = false}},
                                             {0x001F,new ExchangeProperty{ByteCount = 0,Type = 0x001F,MultiValue = true, Variable = true}},
                                             {0x001E,new ExchangeProperty{ByteCount = 0,Type = 0x001E,MultiValue = true, Variable = true}},
                                             {0x0040,new ExchangeProperty{ByteCount = 8,Type = 0x0040,MultiValue = false, Variable = false}},
                                             {0x0048,new ExchangeProperty{ByteCount = 16,Type = 0x0048,MultiValue = false, Variable = false}},
                                             {0x00FB,new ExchangeProperty{ByteCount = 0,Type = 0x00FB,MultiValue = false, Variable = true}},
                                             {0x00FD,new ExchangeProperty{ByteCount = 0,Type = 0x00FD,MultiValue = false, Variable = true}},
                                             {0x00FE,new ExchangeProperty{ByteCount = 0,Type = 0x00FE,MultiValue = true, Variable = true}},
                                             {0x0102,new ExchangeProperty{ByteCount = 1,Type = 0x0102,MultiValue = true, Variable = false}},
                                             {0x1002,new ExchangeProperty{ByteCount = 2,Type = 0x1002,MultiValue = true, Variable = false}},
                                             {0x1003,new ExchangeProperty{ByteCount = 4,Type = 0x1003,MultiValue = true, Variable = false}},
                                             {0x1004,new ExchangeProperty{ByteCount = 4,Type = 0x1004,MultiValue = true, Variable = false}},
                                             {0x1005,new ExchangeProperty{ByteCount = 8,Type = 0x1005,MultiValue = true, Variable = false}},
                                             {0x1006,new ExchangeProperty{ByteCount = 8,Type = 0x1006,MultiValue = true, Variable = false}},
                                             {0x1007,new ExchangeProperty{ByteCount = 8,Type = 0x1007,MultiValue = true, Variable = false}},
                                             {0x1014,new ExchangeProperty{ByteCount = 8,Type = 0x1014,MultiValue = true, Variable = false}},
                                             {0x101F,new ExchangeProperty{ByteCount = 0,Type = 0x101F,MultiValue = true, Variable = true}},
                                             {0x101E,new ExchangeProperty{ByteCount = 0,Type = 0x101E,MultiValue = true, Variable = true}},
                                             {0x1040,new ExchangeProperty{ByteCount = 8,Type = 0x1040,MultiValue = true, Variable = false}},
                                             {0x1048,new ExchangeProperty{ByteCount = 8,Type = 0x1048,MultiValue = true, Variable = false}},
                                             {0x1102,new ExchangeProperty{ByteCount = 0,Type = 0x1102,MultiValue = true, Variable = true}},
                                             //{0x1102,new ExchangeProperty{ByteCount = 0,Type = 0x1102,MultiValue = true, Variable = true}}
                                         };

        public UInt16 ID;
        public UInt16 Type;
        public bool MultiValue;
        public bool Variable;
        public uint ByteCount;
        public byte[] Data;
        private BTHDataEntry entry;

        public ExchangeProperty() {}

        public ExchangeProperty(BTHDataEntry entry, BTH heap)
        {
            this.entry = entry;
            this.ID = BitConverter.ToUInt16(entry.Key, 0);
            this.Type = BitConverter.ToUInt16(entry.Data, 0);
            if (ExchangeProperty.PropertyLookupByTypeID.ContainsKey(this.Type))
            {
                var prop = ExchangeProperty.PropertyLookupByTypeID[this.Type];
                this.MultiValue = prop.MultiValue;
                this.Variable = prop.Variable;
                this.ByteCount = prop.ByteCount;
            }

            //get data here

            if (!this.MultiValue && !this.Variable)
            {
                if (this.ByteCount <= 4)
                {
                    this.Data = entry.Data.RangeSubset(2, (int) this.ByteCount);
                } else
                {
                    this.Data = heap.GetHIDBytes(new HID(entry.Data, 2)).Data;
                }
            } else
            {
                //oh no, it's an HNID
                var curID = BitConverter.ToUInt32(entry.Data,2);

                if (curID == 0)
                {
                    
                } else if ((curID & 0x1F) == 0) //must be HID
                {
                    this.Data = heap.GetHIDBytes(new HID(entry.Data,2)).Data;
                } else //let's assume NID
                {
                    var totalSize = 0;
                    var dataBlocks = new List<BlockDataDTO>();
                    int ii = 0;
                    if (entry.ParentTree.HeapNode.HeapSubNode.ContainsKey(curID))
                        dataBlocks = entry.ParentTree.HeapNode.HeapSubNode[curID].NodeData;
                    else
                    {
                        var tempSubNodeXREF = new Dictionary<ulong, NodeDataDTO>();
                        foreach (var heapSubNode in entry.ParentTree.HeapNode.HeapSubNode)
                            tempSubNodeXREF.Add(heapSubNode.Key & 0xFFFFFFFF, heapSubNode.Value);
                        dataBlocks = tempSubNodeXREF[curID].NodeData;
                        //dataBlocks = entry.ParentTree.HeapNode.HeapSubNode[curID].NodeData;
                    }
                    foreach(var dataBlock in dataBlocks)
                        totalSize += dataBlock.Data.Length;
                    var allData = new byte[totalSize];
                    var curPos = 0;
                    foreach(var datablock in dataBlocks)
                    {
                        for(int i =0; i < datablock.Data.Length;i++)
                        {
                            allData[i + curPos] = datablock.Data[i];
                        }
                        curPos += datablock.Data.Length;
                    }
                    this.Data = allData;
                }
            }
        }
    }
}
