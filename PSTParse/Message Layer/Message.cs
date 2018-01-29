using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.LTP;
using PSTParse.NDB;
using PSTParse.Utilities;

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
    public class Message: IPMItem
    {
        public uint NID { get; set; }
        public NodeDataDTO Data;
        public PropertyContext MessagePC;
        public TableContext AttachmentTable;
        public TableContext RecipientTable;
        public PropertyContext AttachmentPC;

        public String Subject;
        public String SubjectPrefix;
        public Importance Imporance;
        public Sensitivity Sensitivity;
        public DateTime LastSaved;
        
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
        public String CreatorName;
        public UInt32 NonUnicodeCodePage;

        private UInt32 MessageFlags;
        private IPMItem _IPMItem;

        public List<Recipient> To = new List<Recipient>();
        public List<Recipient> From = new List<Recipient>();
        public List<Recipient> CC = new List<Recipient>();
        public List<Recipient> BCC = new List<Recipient>();

        public List<Attachment> Attachments = new List<Attachment>(); 

        public Message(uint NID, IPMItem item, PSTFile pst)
        {
            this._IPMItem = item;
            this.Data = BlockBO.GetNodeData(NID, pst);
            this.NID = NID;
            //this.MessagePC = new PropertyContext(this.Data);
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
                        this.Attachments = new List<Attachment>();
                        foreach(var row in this.AttachmentTable.RowMatrix.Rows)
                        {
                            this.Attachments.Add(new Attachment(row));
                        }
                        break;
                    case NDB.NID.NodeType.RECIPIENT_TABLE:
                        this.RecipientTable = new TableContext(subNode.Value);
                        
                        foreach(var row in this.RecipientTable.RowMatrix.Rows)
                        {
                            var recipient = new Recipient(row);
                            switch(recipient.Type)
                            {
                                case Recipient.RecipientType.TO:
                                    this.To.Add(recipient);
                                    break;
                                case Recipient.RecipientType.FROM:
                                    this.From.Add(recipient);
                                    break;
                                case Recipient.RecipientType.CC:
                                    this.CC.Add(recipient);
                                    break;
                                case Recipient.RecipientType.BCC:
                                    this.BCC.Add(recipient);
                                    break;
                            }
                        }
                        break;
                }
            }
            foreach(var prop in this._IPMItem.PC.Properties)
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
                        this.Subject = Encoding.Unicode.GetString(prop.Value.Data);
                        if (this.Subject.Length > 0)
                        {
                            var chars = this.Subject.ToCharArray();
                            if (chars[0] == 0x001)
                            {
                                var length = (int)chars[1];
                                int i = 0;
                                if (length > 1)
                                    i++;
                                this.SubjectPrefix = this.Subject.Substring(2, length-1);
                                this.Subject = this.Subject.Substring(2 + length-1);
                            }
                        }
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
                    case 0x3ff1:
                        //localeID
                        break;
                    case 0x3ff8:
                        this.CreatorName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case 0x3ff9:
                        //creator entryid
                        break;
                    case 0x3ffa:
                        //last modifier name
                        break;
                    case 0x3ffb:
                        //last modifier entryid
                        break;
                    case 0x3ffd:
                        this.NonUnicodeCodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0x4019:
                        //unknown
                        break;
                    case 0x401a:
                        //sentrepresentingflags
                        break;
                    case 0x619:
                        //userentryid
                        break;
                    default:
                        break;
                }
            }
            
        }
    }
}
