using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*namespace PSTParse.LTP
{
    public class BTHIndexAllocationRecords
    {
        public List<BTHIndexNode> BTHIndexRecords;
        public int CurrentLevel;
        public BTHIndexAllocationRecords(byte[] bytes, int offset, BTHHEADER header, int level)
        {
            this.CurrentLevel = level;
            this.BTHIndexRecords = new List<BTHIndexNode>();

            var keySize = (int)header.KeySize;
            for (int i = offset; i < bytes.Length; i += 4 + keySize)
                this.BTHIndexRecords.Add(new BTHIndexNode(bytes, header, i));
        }
    }
}*/
