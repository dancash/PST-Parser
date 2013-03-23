using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NDB
{
    /*
    public static class BlockFactory
    {
        public static IBLOCK GetBlock(byte[] bytes, int blockDataSize, 
            bool isInternal, bool isSubNode)
        {
            var trailerOffset = bytes.Length - 16;
            var trailer = new BlockTrailer(bytes, trailerOffset);

            

            if (!isInternal)
                return new DataBlock(bytes, blockDataSize);
            else
            {
                var bType = bytes[0];
                var headerLevel = bytes[1];

                if (isSubNode)
                {
                    if (headerLevel == 0)
                        return new SLBLOCK(bytes);
                    else
                        return new SIBLOCK(bytes);
                } else
                {
                    if (headerLevel == 1)
                        return new XBLOCK(bytes);
                    else
                        return new XXBLOCK(bytes);
                }
            }
        }
    }*/
}
