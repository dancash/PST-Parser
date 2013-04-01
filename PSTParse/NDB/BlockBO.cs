using System;
using System.Collections.Generic;

namespace PSTParse.NDB
{
    public static class BlockBO
    {
        public static NodeDataDTO GetNodeData(ulong nid)
        {
            var pst = PSTFile.CurPST;
            var nodeBIDs = pst.GetNodeBIDs(nid);
            var mainData = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(nodeBIDs.Item1));
            var subNodeData = new Dictionary<ulong,NodeDataDTO>();

            if (nodeBIDs.Item2 != 0)
                subNodeData = BlockBO.GetSubNodeData(pst.GetBlockBBTEntry(nodeBIDs.Item2));

            return new NodeDataDTO {NodeData = mainData, SubNodeData = subNodeData};
        }

        private static Dictionary<ulong, NodeDataDTO> GetSubNodeData(BBTENTRY entry)
        {
            var allData = BlockBO.GetBBTEntryData(entry);
            var dataBlock = allData[0];
            if (entry.Internal)
            {
                var type = dataBlock.Data[0];
                var cLevel = dataBlock.Data[1];
                if (cLevel == 0) //SLBlock, no intermediate
                {
                    return BlockBO.GetSLBlockData(new SLBLOCK(dataBlock));
                } else //SIBlock
                {
                    return BlockBO.GetSIBlockData(new SIBLOCK(dataBlock));
                }
            } else
            {
                throw new Exception("Whoops");
            }
        }

        private static Dictionary<ulong, NodeDataDTO> GetSIBlockData(SIBLOCK siblock)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();

            foreach(var entry in siblock.Entries)
            {
                var curSLBlockBBT = PSTFile.CurPST.GetBlockBBTEntry(entry.SLBlockBID);
                var slblock = new SLBLOCK(BlockBO.GetBBTEntryData(curSLBlockBBT)[0]);
                var data = BlockBO.GetSLBlockData(slblock);
                foreach(var item in data)
                    ret.Add(item.Key, item.Value);
            }
            
            return ret;
        }
        //gets all the data for an SL block.  an SL block points directly to all the immediate subnodes
        private static Dictionary<ulong, NodeDataDTO> GetSLBlockData(SLBLOCK slblock)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();
            foreach(var entry in slblock.Entries)
            {
                //this data should represent the main data part of the subnode
                var data = BlockBO.GetBBTEntryData(PSTFile.CurPST.GetBlockBBTEntry(entry.SubNodeBID));
                var cur = new NodeDataDTO {NodeData = data};
                ret.Add(entry.SubNodeNID, cur);

                //see if there are sub nodes of this current sub node
                if (entry.SubSubNodeBID != 0)
                    //if there are subnodes, treat them like any other subnode
                    cur.SubNodeData = GetSubNodeData(PSTFile.CurPST.GetBlockBBTEntry(entry.SubSubNodeBID));

                
            }
            return ret;
        }
        public static NodeDataDTO GetNodeData(NBTENTRY entry)
        {
            var mainData = BlockBO.GetBBTEntryData(PSTFile.CurPST.GetBlockBBTEntry(entry.BID_Data));
            if (entry.BID_SUB != 0)
            {
                var subnodeData = BlockBO.GetSubNodeData(PSTFile.CurPST.GetBlockBBTEntry(entry.BID_SUB));
                return new NodeDataDTO {NodeData = mainData, SubNodeData = subnodeData};
            } 

            return new NodeDataDTO {NodeData = mainData, SubNodeData = null};
        }

        public static NodeDataDTO GetNodeData(SLENTRY entry)
        {
            var mainData = BlockBO.GetBBTEntryData(PSTFile.CurPST.GetBlockBBTEntry(entry.SubNodeBID));
            if (entry.SubSubNodeBID != 0)
            {
                var subNodeData = BlockBO.GetSubNodeData(PSTFile.CurPST.GetBlockBBTEntry(entry.SubSubNodeBID));
                return new NodeDataDTO {NodeData = mainData, SubNodeData = subNodeData};
            }

            return new NodeDataDTO {NodeData = mainData, SubNodeData = null};
        }

        //for a given bbt entry, retrieve the raw bytes associated with the BID
        //this includes retrieving data trees via xblocks
        public static List<BlockDataDTO> GetBBTEntryData(BBTENTRY entry)
        {
            var dataSize = entry.BlockByteCount;
            var blockSize = entry.BlockByteCount + 16;
            if (blockSize % 64 != 0)
                blockSize += 64 - (blockSize%64);
            List<BlockDataDTO> dataBlocks;

            /*if (isSubNode)
            {
                using (var viewer = PSTFile.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, blockSize))
                {
                    var blockBytes = new byte[dataSize];
                    viewer.ReadArray(0, blockBytes, 0, dataSize);
                    dataBlocks = new List<BlockDataDTO>
                                     {new BlockDataDTO {Data = blockBytes, PstOffset = entry.BREF.IB, BBTEntry = entry}};
                    return dataBlocks;
                }
            } else */
            if (entry.Internal)
            {
                using(var viewer = PSTFile.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,blockSize))
                {
                    var blockBytes = new byte[dataSize];
                    viewer.ReadArray(0, blockBytes, 0, dataSize);

                    var trailerBytes = new byte[16];
                    viewer.ReadArray(blockSize-16, trailerBytes, 0, 16);
                    var trailer = new BlockTrailer(trailerBytes, 0);
                    
                    var dataBlockDTO = new BlockDataDTO
                                           {
                                               Data = blockBytes,
                                               PstOffset = entry.BREF.IB,
                                               CRCOffset = (uint)((long)entry.BREF.IB + (blockSize - 12)),
                                               BBTEntry = entry
                                           };
                    var type = blockBytes[0];
                    var level = blockBytes[1];

                    if (type == 2) //si or sl entry
                    {
                        return new List<BlockDataDTO> {dataBlockDTO};
                    } else if (type == 1)
                    {
                        if (blockBytes[1] == 0x01) //XBLOCK
                        {
                            var xblock = new XBLOCK(dataBlockDTO);
                            return BlockBO.GetXBlockData(xblock);
                        
                        } else //XXBLOCK
                        {
                            var xxblock = new XXBLOCK(dataBlockDTO);
                            return BlockBO.GetXXBlockData(xxblock);
                        }
                    } else
                    {
                        throw new NotImplementedException();
                    }
                }
            } else
            {
                using(var viewer = PSTFile.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,blockSize))
                {
                    var dataBytes = new byte[dataSize];
                    viewer.ReadArray(0, dataBytes, 0, dataSize);
                    
                    var trailerBytes = new byte[16];
                    viewer.ReadArray(blockSize-16, trailerBytes, 0, 16);
                    var trailer = new BlockTrailer(trailerBytes, 0);
                    dataBlocks = new List<BlockDataDTO>
                                     {
                                         new BlockDataDTO
                                             {
                                                 Data = dataBytes,
                                                 PstOffset = entry.BREF.IB,
                                                 CRC32 = trailer.CRC,
                                                 CRCOffset = (uint) (blockSize -12),
                                                 BBTEntry = entry
                                             }
                                     };
                }
            }

            for (int i = 0; i < dataBlocks.Count; i++)
            {
                var temp = dataBlocks[i].Data;   
                DatatEncoder.CryptPermute(temp, temp.Length, false);
            }
            return dataBlocks;
        }

        private static List<BlockDataDTO> GetXBlockData(XBLOCK xblock)
        {
            var ret = new List<BlockDataDTO>();
            foreach(var bid in xblock.BIDEntries)
            {
                var bbtEntry = PSTFile.CurPST.GetBlockBBTEntry(bid);
                ret.AddRange(BlockBO.GetBBTEntryData(bbtEntry));
            }
            return ret;
        }

        private static List<BlockDataDTO> GetXXBlockData(XXBLOCK xxblock)
        {
            var ret = new List<BlockDataDTO>();
            foreach(var bid in xxblock.XBlockBIDs)
            {
                var bbtEntry = PSTFile.CurPST.GetBlockBBTEntry(bid);
                var curXblockData = BlockBO.GetBBTEntryData(bbtEntry);
                //var curXblockData = BlockBO.GetXBlockData(curXblock);
                foreach(var block in curXblockData)
                    ret.Add(block);
            }
            return ret;
        }
    }
}
