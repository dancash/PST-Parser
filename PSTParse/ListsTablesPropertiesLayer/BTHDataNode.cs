using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PSTParse.NodeDatabaseLayer;
using PSTParse.Utilities;

namespace PSTParse.ListsTablesPropertiesLayer
{
    public class BTHDataNode
    {
        public List<BTHDataEntry> DataEntries;
        private HNDataDTO _data;
        public BTH Tree;

        public BTHDataNode(HID hid, BTH tree)
        {
            this.Tree = tree;

            var bytes = tree.GetHIDBytes(hid);
            this._data = bytes;
            this.DataEntries = new List<BTHDataEntry>();
            for(int i= 0;i < bytes.Data.Length;i+= (int)(tree.Header.KeySize+tree.Header.DataSize))
                this.DataEntries.Add(new BTHDataEntry(bytes, i, tree));
        }

        //this is only here for testing purposes, this needs to be moved
        public bool BlankPassword(PSTFile pst)
        {
            var toMatch = new byte[] {0xFF, 0x67};
            foreach (var entry in this.DataEntries)
                if (entry.Key[0] == toMatch[0] && entry.Key[1] == toMatch[1])
                {
                    pst.CloseMMF();
                    //DatatEncoder.CryptPermute(ref this._data.Parent.Data, this._data.Parent.Data.Length, true);

                    using (var stream = new FileStream(pst.Path, FileMode.Open))
                    {
                        var dataBlockOffset = entry.DataOffset;

                        //this._data.Parent.Data[dataBlockOffset] = 0x00;
                        //this._data.Parent.Data[dataBlockOffset + 1] = 0x00;
                        this._data.Parent.Data[dataBlockOffset + 2] = 0x00;
                        this._data.Parent.Data[dataBlockOffset + 3] = 0x00;
                        this._data.Parent.Data[dataBlockOffset + 4] = 0x00;
                        this._data.Parent.Data[dataBlockOffset + 5] = 0x00;

                        DatatEncoder.CryptPermute(this._data.Parent.Data, this._data.Parent.Data.Length, true, pst);

                        var testCRC = (new CRC32()).ComputeCRC(0, this._data.Parent.Data, (uint)this._data.Parent.Data.Length);
                        stream.Seek((long)(this._data.Parent.PstOffset + entry.DataOffset), SeekOrigin.Begin);

                        stream.Write(
                            new []
                                {
                                    //this._data.Parent.Data[dataBlockOffset],
                                    //this._data.Parent.Data[dataBlockOffset + 1],
                                    this._data.Parent.Data[dataBlockOffset + 2],
                                    this._data.Parent.Data[dataBlockOffset + 3],
                                    this._data.Parent.Data[dataBlockOffset + 4],
                                    this._data.Parent.Data[dataBlockOffset + 5]
                                }, 0, 4);

                        var newCRC = (new CRC32()).ComputeCRC(0, this._data.Parent.Data, (uint) this._data.Parent.Data.Length);

                        DatatEncoder.CryptPermute(this._data.Parent.Data, this._data.Parent.Data.Length, false, pst);
                        var crcoffset = (int) (this._data.Parent.PstOffset + this._data.Parent.CRCOffset);
                        stream.Seek(crcoffset, SeekOrigin.Begin);
                        var temp = BitConverter.GetBytes(newCRC);
                        stream.Write(new []
                                         {
                                             temp[0],temp[1],temp[2],temp[3]
                                         }, 0, 4);
                        
                    }

                    pst.OpenMMF();
                    return true;
                }
            
            return false;
        }
    }
}
