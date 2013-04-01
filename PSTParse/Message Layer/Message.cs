using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public class Message
    {
        public uint NID { get; set; }
        public NodeDataDTO Data;
        public PropertyContext MessagePC;
        public TableContext AttachmentTable;
        public TableContext RecipientTable;
        public PropertyContext AttachmentPC;

        public Message(uint NID)
        {
            int i = 0;
            if (NID == 0x20e324)
                i++;
            this.Data = BlockBO.GetNodeData(NID);
            this.NID = NID;
            this.MessagePC = new PropertyContext(this.Data);
            foreach(var subNode in this.Data.SubNodeData)
            {
                var temp = new NID(subNode.Key);
                switch(temp.Type)
                {
                    case NDB.NID.NodeType.ATTACHMENT_TABLE:
                        this.AttachmentTable = new TableContext(subNode.Value);
                        break;
                    case NDB.NID.NodeType.ATTACHMENT_PC:
                        this.AttachmentPC = new PropertyContext(subNode.Value);
                        break;
                    case NDB.NID.NodeType.RECIPIENT_TABLE:
                        this.RecipientTable = new TableContext(subNode.Value);
                        break;
                }
            }
        }
    }
}
