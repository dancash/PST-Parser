namespace PSTParse.NodeDatabaseLayer
{
    public class NID
    {
        public enum NodeType
        {
            //heap node
            HID = 0x00,
            INTERNAL = 0x01,
            NORMAL_FOLDER = 0x02,
            SEARCH_FOLDER = 0x03,
            NORMAL_MESSAGE_PC = 0x03,
            ATTACHMENT_PC = 0x05,
            // queue of changed objects for search folder object
            SEARCH_UPDATE_QUEUE = 0x06,
            SEARCH_CRITERIA_OBJECT = 0x07,
            ASSOC_MESSAGE = 0X08,
            CONTENTS_TABLE_INDEX = 0X0A,
            //inbox
            RECEIVE_FOLDER_TABLE = 0X0B,
            //outbox
            OUTGOING_QUEUE_TABLE = 0X0C,
            HIERARCHY_TABLE = 0X0D,
            CONTENTS_TABLE = 0X0E,
            ASSOC_CONTENTS_TABLE = 0X0F,
            SEARCH_CONTENTS_TABLE = 0X10,
            ATTACHMENT_TABLE = 0X11,
            RECIPIENT_TABLE = 0X12,
            SEARCH_TABLE_INDEX = 0X13,
            LTP = 0X14
        }
        public static NodeType GetNodeType(ulong nid) => (NodeType)(nid & 0x1f);
    }
}
