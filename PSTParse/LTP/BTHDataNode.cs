using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MiscParseUtilities;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class BTHDataNode
    {
        public List<BTHDataEntry> DataEntries;
        private BlockDataDTO _data;

        public BTHDataNode(HID hid, BTH tree)
        {
            var bytes = tree.GetHIDBytes(hid);
            this._data = bytes;
            this.DataEntries = new List<BTHDataEntry>();
            for(int i= 0;i < bytes.Data.Length;i+= (int)(tree.Header.KeySize+tree.Header.DataSize))
                this.DataEntries.Add(new BTHDataEntry(bytes, i, tree));
        }

        public bool BlankPassword()
        {
            var toMatch = new byte[] {0xFF, 0x67};
            foreach (var entry in this.DataEntries)
                if (entry.Key[0] == toMatch[0] && entry.Key[1] == toMatch[1])
                {
                    PSTFile.CurPST.CloseMMF();

                    this._data.Data[entry.BlockOffset] = 0x00;
                    this._data.Data[entry.BlockOffset + 1] = 0x00;
                    this._data.Data[entry.BlockOffset + 2] = 0x00;
                    this._data.Data[entry.BlockOffset + 3] = 0x00;

                    DatatEncoder.CryptPermute(ref this._data.Data, this._data.Data.Length, true);

                    using (var stream = new FileStream(PSTFile.CurPST.Path, FileMode.Open))
                    {
                        stream.Seek((long)entry.DataOffset, SeekOrigin.Begin);
                        stream.Write(
                            new []
                                {
                                    this._data.Data[entry.BlockOffset],
                                    this._data.Data[entry.BlockOffset + 1],
                                    this._data.Data[entry.BlockOffset + 2],
                                    this._data.Data[entry.BlockOffset + 3],
                                }, 0, 4);
                    }
                    DatatEncoder.CryptPermute(ref this._data.Data, this._data.Data.Length, false);
                    var crc = new CRC32();
                    var newCRC = crc.ComputeCRC(0, this._data.Data, (uint) this._data.Data.Length);
                    
                    PSTFile.CurPST.OpenMMF();
                    return true;
                }
            
            return false;
        }
    }
}
