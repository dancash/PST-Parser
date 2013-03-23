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
            var dataBlock = GetBBTEntryData(entry)[0];
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
                var slblock = new SLBLOCK(BlockBO.GetBBTEntryData(PSTFile.CurPST.GetBlockBBTEntry(entry.SLBlockBID))[0]);
                var data = BlockBO.GetSLBlockData(slblock);
                foreach(var item in data)
                    ret.Add(item.Key, item.Value);
            }
            
            return ret;
        }

        private static Dictionary<ulong, NodeDataDTO> GetSLBlockData(SLBLOCK slblock)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();
            foreach(var entry in slblock.Entries)
            {
                var data = BlockBO.GetBBTEntryData(PSTFile.CurPST.GetBlockBBTEntry(entry.SubNodeBID));
                var cur = new NodeDataDTO {NodeData = data};
                if (entry.SubSubNodeBID != 0)
                    cur.SubNodeData = GetSubNodeData(PSTFile.CurPST.GetBlockBBTEntry(entry.SubNodeBID));
                ret.Add(entry.SubNodeNID, cur);
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

        public static List<BlockDataDTO> GetBBTEntryData(BBTENTRY entry)
        {
            var dataSize = entry.BlockByteCount;
            var blockSize = entry.BlockByteCount + 16;
            if (blockSize % 64 != 0)
                blockSize += 64 - (blockSize%64);
            List<BlockDataDTO> dataBlocks;

            if (entry.Internal)
            {
                using(var viewer = PSTFile.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,blockSize))
                {
                    var blockBytes = new byte[blockSize];
                    viewer.ReadArray(0, blockBytes, 0, blockSize);
                    var dataBlockDTO = new BlockDataDTO {Data = blockBytes, PstOffset = entry.BREF.IB};
                    
                    if (blockBytes[1] == 0x01) //XBLOCK
                    {
                        var xblock = new XBLOCK(dataBlockDTO);
                        dataBlocks = BlockBO.GetXBlockData(xblock);
                        
                    } else //XXBLOCK
                    {
                        var xxblock = new XXBLOCK(dataBlockDTO);
                        dataBlocks = BlockBO.GetXXBlockData(xxblock);
                    }
                }
            } else
            {
                using(var viewer = PSTFile.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,blockSize))
                {
                    var dataBytes = new byte[dataSize];
                    viewer.ReadArray(0, dataBytes, 0, dataSize);
                    var padSize = (dataSize + 16)%64;
                    if (padSize != 0)
                        padSize = 64 - padSize;
                    var trailerBytes = new byte[16];
                    viewer.ReadArray(dataSize + padSize, trailerBytes, 0, 16);
                    var trailer = new BlockTrailer(trailerBytes, 0);
                    //var storedHash = trailer.CRC;
                    //var crc = new CRC32();
                    //DatatEncoder.CryptPermute(ref dataBytes, dataSize, false);
                    //var hash = crc.ComputeCRC(0, dataBytes, dataSize);
                    dataBlocks = new List<BlockDataDTO>
                                     {
                                         new BlockDataDTO
                                             {
                                                 Data = dataBytes,
                                                 PstOffset = entry.BREF.IB,
                                                 CRC32 = trailer.CRC,
                                                 CRCOffset = (uint) (dataSize + padSize + 4)
                                             }
                                     };
                }
            }

            for (int i = 0; i < dataBlocks.Count; i++)
            {
                var temp = dataBlocks[i].Data;
                
                DatatEncoder.CryptPermute(ref temp, temp.Length, false);
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
                var curXblock = new XBLOCK(BlockBO.GetBBTEntryData(bbtEntry)[0]);
                var curXblockData = BlockBO.GetXBlockData(curXblock);
                ret.AddRange(curXblockData);
            }
            return ret;
        }
    }
}
