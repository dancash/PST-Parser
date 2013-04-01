using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class MailFolder : IEnumerable<Message>
    {
        public PropertyContext PC;
        public TableContext HeirachyTC;
        public TableContext ContentsTC;
        public TableContext FaiTC;

        public string DisplayName;
        public List<string> Path;

        public List<MailFolder> SubFolders; 

        public MailFolder(ulong NID, List<string> path)
        {
            this.Path = path;
            var nid = NID;
            var pcNID = ((nid >> 5) << 5) | 0x02;
            this.PC = new PropertyContext(pcNID);
            this.DisplayName = Encoding.Unicode.GetString(this.PC.Properties[0x3001].Data);

            this.Path = new List<string>(path);
            this.Path.Add(DisplayName);

            var heirachyNID = ((nid >> 5) << 5) | 0x0D;
            var contentsNID = ((nid >> 5) << 5) | 0x0E;
            var faiNID = ((nid >> 5) << 5) | 0x0F;

            this.HeirachyTC = new TableContext(heirachyNID);

            this.SubFolders = new List<MailFolder>();
            foreach(var row in this.HeirachyTC.ReverseRowIndex)
            {
                this.SubFolders.Add(new MailFolder(row.Value, this.Path));
                //var temp = row.Key;
                //var temp2 = row.Value;
                //this.SubFolderEntryIDs.Add(row.);
            }
            
            this.ContentsTC = new TableContext(contentsNID);

            
            this.FaiTC = new TableContext(faiNID);
        }

        public IEnumerator<Message> GetEnumerator()
        {
            foreach(var row in this.ContentsTC.ReverseRowIndex)
            {
                yield return new Message(row.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
