using System.Collections.Generic;
using System.Text;
using PSTParse.ListsTablesPropertiesLayer;

namespace PSTParse.MessageLayer
{
    public class MailFolder
    {
        private readonly PSTFile _pst;
        private readonly PropertyContext PC;
        private readonly TableContext HeirachyTC;
        private readonly TableContext ContentsTC;
        private readonly TableContext FaiTC;

        public string DisplayName { get; }
        public string ContainerClass { get; }
        public List<string> Path { get; }
        public List<MailFolder> SubFolders { get; }
        public int Count => ContentsTC.RowIndexBTH.Properties.Count;

        public MailFolder(ulong nid, List<string> path, PSTFile pst)
        {
            _pst = pst;

            Path = path;
            var pcNID = ((nid >> 5) << 5) | 0x02;
            PC = new PropertyContext(pcNID, pst);
            DisplayName = Encoding.Unicode.GetString(PC.Properties[(MessageProperty)0x3001].Data);


            PC.Properties.TryGetValue((MessageProperty)0x3613, out ExchangeProperty containerClassProperty);
            ContainerClass = containerClassProperty == null ? "" : Encoding.Unicode.GetString(containerClassProperty.Data);

            Path = new List<string>(path) { DisplayName };

            var heirachyNID = ((nid >> 5) << 5) | 0x0D;
            var contentsNID = ((nid >> 5) << 5) | 0x0E;
            var faiNID = ((nid >> 5) << 5) | 0x0F;

            HeirachyTC = new TableContext(heirachyNID, pst);

            SubFolders = new List<MailFolder>();
            foreach (var row in HeirachyTC.ReverseRowIndex)
            {
                SubFolders.Add(new MailFolder(row.Value, Path, pst));
                //var temp = row.Key;
                //var temp2 = row.Value;
                //SubFolderEntryIDs.Add(row.);
            }

            ContentsTC = new TableContext(contentsNID, pst);


            FaiTC = new TableContext(faiNID, pst);
        }

        public IEnumerable<IPMItem> GetIpmItems()
        {
            foreach (var row in ContentsTC.ReverseRowIndex)
            {
                var propertyContext = new PropertyContext(row.Value, _pst);
                if (propertyContext.MessageClassProperty == "IPM.Note")
                    yield return new Message(_pst, propertyContext);
                else
                    yield return new IPMItem(_pst, propertyContext);
            }
        }

        public IEnumerable<Message> GetIpmNotes()
        {
            foreach (var row in ContentsTC.ReverseRowIndex)
            {
                var propertyContext = new PropertyContext(row.Value, _pst);
                if (propertyContext.MessageClassProperty == "IPM.Note")
                    yield return new Message(_pst, propertyContext);
            }
        }
    }
}
