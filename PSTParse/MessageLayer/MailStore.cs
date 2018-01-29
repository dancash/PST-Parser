using PSTParse.ListsTablesPropertiesLayer;
using PSTParse.NodeDatabaseLayer;

namespace PSTParse.MessageLayer
{
    public class MailStore
    {
        public EntryID RootFolder;
        private PropertyContext _pc;

        public MailStore(PSTFile pst)
        {
            this._pc = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE, pst);
            this.RootFolder = new EntryID(this._pc.BTH.GetExchangeProperties()[0x35e0].Data);
        }
    }
}
