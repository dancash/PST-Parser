using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.LTP
{
    public class BTHDataRecord
    {
        public uint Key;
        public uint Value;

        public BTHDataRecord(byte[] bytes, BTHHEADER header)
        {
            var keySize = (int)header.KeySize;
            var dataSize = (int) header.DataSize;

            this.Key = BitConverter.ToUInt16(bytes.Take(keySize).ToArray(), 0);
            this.Value = BitConverter.ToUInt32(bytes.Skip(keySize).Take(dataSize).ToArray(), 0);
        }
    }
}
