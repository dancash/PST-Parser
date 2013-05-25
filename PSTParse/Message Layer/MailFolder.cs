using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class MailFolder : IEnumerable<IPMItem>
    {
        public PropertyContext PC;
        public TableContext HeirachyTC;
        public TableContext ContentsTC;
        public TableContext FaiTC;

        public string DisplayName;
        public List<string> Path;

        public List<MailFolder> SubFolders;

        private PSTFile _pst;

        public MailFolder(ulong NID, List<string> path, PSTFile pst)
        {
            this._pst = pst;

            this.Path = path;
            var nid = NID;
            var pcNID = ((nid >> 5) << 5) | 0x02;
            this.PC = new PropertyContext(pcNID, pst);
            this.DisplayName = Encoding.Unicode.GetString(this.PC.Properties[0x3001].Data);

            this.Path = new List<string>(path);
            this.Path.Add(DisplayName);

            var heirachyNID = ((nid >> 5) << 5) | 0x0D;
            var contentsNID = ((nid >> 5) << 5) | 0x0E;
            var faiNID = ((nid >> 5) << 5) | 0x0F;

            this.HeirachyTC = new TableContext(heirachyNID, pst);

            this.SubFolders = new List<MailFolder>();
            foreach(var row in this.HeirachyTC.ReverseRowIndex)
            {
                this.SubFolders.Add(new MailFolder(row.Value, this.Path, pst));
                //var temp = row.Key;
                //var temp2 = row.Value;
                //this.SubFolderEntryIDs.Add(row.);
            }
            
            this.ContentsTC = new TableContext(contentsNID, pst);

            
            this.FaiTC = new TableContext(faiNID, pst);
        }

        public IEnumerator<IPMItem> GetEnumerator()
        {
            foreach(var row in this.ContentsTC.ReverseRowIndex)
            {
                var curItem = new IPMItem(this._pst, row.Value);
                //if (curItem.MessageClass.StartsWith("IPM.Note"))
                    yield return new Message(row.Value, curItem, this._pst);
                /*else
                    yield return curItem;*/
                //yield return new Message(row.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
