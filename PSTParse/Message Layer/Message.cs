using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscParseUtilities;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public enum Importance
    {
        LOW = 0x00,
        NORMAL = 0x01,
        HIGH = 0x02
    }

    public enum Sensitivity
    {
        Normal = 0x00,
        Personal = 0x01,
        Private = 0x02,
        Confidential = 0x03
    }
    public class Message
    {
        public uint NID { get; set; }
        public NodeDataDTO Data;
        public PropertyContext MessagePC;
        public TableContext AttachmentTable;
        public TableContext RecipientTable;
        public PropertyContext AttachmentPC;

        public String Subject;
        public Importance Imporance;
        public Sensitivity Sensitivity;
        public DateTime LastSaved;
        public String MessageClass;
        public DateTime ClientSubmitTime;
        public string SentRepresentingName;
        public string ConversationTopic;
        public string SenderName;
        public DateTime MessageDeliveryTime;
        public Boolean Read;
        public Boolean Unsent;
        public Boolean Unmodified;
        public Boolean HasAttachments;
        public Boolean FromMe;
        public Boolean IsFAI;
        public Boolean NotifyReadRequested;
        public Boolean NotifyUnreadRequested;
        public Boolean EverRead;
        public UInt32 MessageSize;
        public string BodyPlainText;
        public UInt32 InternetArticalNumber;
        public byte[] BodyCompressedRTF;
        public string InternetMessageID;
        public string UrlCompositeName;
        public bool AttributeHidden;
        public bool ReadOnly;
        public DateTime CreationTime;
        public DateTime LastModificationTime;
        public UInt32 CodePage;

        private UInt32 MessageFlags;

        public Message(uint NID)
        {
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
            foreach(var prop in this.MessagePC.Properties)
            {
                if (prop.Value.Data == null)
                    continue;
                switch(prop.Key)
                {
                    case 0x17:
                        this.Imporance = (Importance) BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case 0x36:
                        this.Sensitivity = (Sensitivity) BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case 0x37:
                        this.Subject = Encoding.Unicode.GetString(prop.Value.Data, 4, prop.Value.Data.Length - 4);
                        break;
                    case 0x39:
                        this.ClientSubmitTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x42:
                        this.SentRepresentingName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x70:
                        this.ConversationTopic = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x1a:
                        this.MessageClass = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0xc1a:
                        this.SenderName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0xe06:
                        this.MessageDeliveryTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0xe07:
                        this.MessageFlags = BitConverter.ToUInt32(prop.Value.Data, 0);

                        this.Read = (this.MessageFlags & 0x1) != 0;
                        this.Unsent = (this.MessageFlags & 0x8) != 0;
                        this.Unmodified = (this.MessageFlags & 0x2) != 0;
                        this.HasAttachments = (this.MessageFlags & 0x10) != 0;
                        this.FromMe = (this.MessageFlags & 0x20) != 0;
                        this.IsFAI = (this.MessageFlags & 0x40) != 0;
                        this.NotifyReadRequested = (this.MessageFlags & 0x100) != 0;
                        this.NotifyUnreadRequested = (this.MessageFlags & 0x200) != 0;
                        this.EverRead = (this.MessageFlags & 0x400) != 0;
                        break;
                    case 0xe08:
                        this.MessageSize = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0xe23:
                        this.InternetArticalNumber = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0xe27:
                        //unknown
                        break;
                    case 0xe29:
                        //nextSentAccount, ignore this, string
                        break;
                    case 0xe62:
                        //unknown
                        break;
                    case 0xe79:
                        //trusted sender
                        break;
                    case 0x1000:
                        this.BodyPlainText = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x1009:
                        this.BodyCompressedRTF = prop.Value.Data.RangeSubset(4, prop.Value.Data.Length - 4);
                        break;
                    case 0x1035:
                        this.InternetMessageID = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x10F3:
                        this.UrlCompositeName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x10F4:
                        this.AttributeHidden = prop.Value.Data[0] == 0x01;
                        break;
                    case 0x10F5:
                        //unknown
                        break;
                    case 0x10F6:
                        this.ReadOnly = prop.Value.Data[0] == 0x01;
                        break;
                    case 0x3007:
                        this.CreationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x3008:
                        this.LastModificationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x300B:
                        //seach key
                        break;
                    case 0x3fDE:
                        this.CodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    default:
                        break;
                }
            }
            
        }
    }
}
