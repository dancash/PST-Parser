using System;
using System.Collections.Generic;
using System.Text;
using PSTParse.ListsTablesPropertiesLayer;
using PSTParse.NodeDatabaseLayer;
using PSTParse.Utilities;

namespace PSTParse.MessageLayer
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

    public class Message : IPMItem
    {
        public uint NID { get; set; }
        public NodeDataDTO Data;
        public PropertyContext MessagePC;
        //public TableContext AttachmentTable { get; set; }
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
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderAddressType { get; set; }
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
        public string Headers { get; set; }
        public string BodyPlainText { get; set; }
        public string BodyHtml { get; set; }
        public UInt32 InternetArticleNumber;
        public string BodyCompressedRTFString { get; set; }
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

        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        public Message(uint nid, IPMItem item, PSTFile pst)
        {
            _IPMItem = item;
            Data = BlockBO.GetNodeData(nid, pst);
            NID = nid;
            //MessagePC = new PropertyContext(Data);
            //int attachmentPcIndex = 0;
            var attachmentSet = new HashSet<string>();

            foreach (var subNode in Data.SubNodeData)
            {
                var temp = new NID(subNode.Key);
                switch (temp.Type)
                {
                    case NodeDatabaseLayer.NID.NodeType.ATTACHMENT_TABLE:
                        //AttachmentTable = new TableContext(subNode.Value);
                        break;
                    case NodeDatabaseLayer.NID.NodeType.ATTACHMENT_PC:
                        AttachmentPC = new PropertyContext(subNode.Value);
                        var attachment = new Attachment(AttachmentPC);
                        if (attachmentSet.Contains(attachment.AttachmentLongFileName))
                        {
                            var smallGuid = Guid.NewGuid().ToString().Substring(0, 5);
                            attachment.AttachmentLongFileName = $"{smallGuid}-{attachment.AttachmentLongFileName}";
                        }
                        attachmentSet.Add(attachment.AttachmentLongFileName);
                        Attachments.Add(attachment);
                        break;
                    case NodeDatabaseLayer.NID.NodeType.RECIPIENT_TABLE:
                        RecipientTable = new TableContext(subNode.Value);

                        foreach (var row in RecipientTable.RowMatrix.Rows)
                        {
                            var recipient = new Recipient(row);
                            switch (recipient.Type)
                            {
                                case Recipient.RecipientType.TO:
                                    To.Add(recipient);
                                    break;
                                case Recipient.RecipientType.FROM:
                                    From.Add(recipient);
                                    break;
                                case Recipient.RecipientType.CC:
                                    CC.Add(recipient);
                                    break;
                                case Recipient.RecipientType.BCC:
                                    BCC.Add(recipient);
                                    break;
                            }
                        }
                        break;
                }
            }
            foreach (var prop in _IPMItem.PC.Properties)
            {
                if (prop.Value.Data == null)
                    continue;
                switch (prop.Key)
                {
                    case MessageProperty.Importance:
                        Imporance = (Importance)BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case MessageProperty.Sensitivity:
                        Sensitivity = (Sensitivity)BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case MessageProperty.Subject:
                        Subject = Encoding.Unicode.GetString(prop.Value.Data);
                        if (Subject.Length > 0)
                        {
                            var chars = Subject.ToCharArray();
                            if (chars[0] == 0x001)
                            {
                                var length = (int)chars[1];
                                int i = 0;
                                if (length > 1)
                                    i++;
                                SubjectPrefix = Subject.Substring(2, length - 1);
                                Subject = Subject.Substring(2 + length - 1);
                            }
                        }
                        break;
                    case MessageProperty.ClientSubmitTime:
                        ClientSubmitTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.SentRepresentingName:
                        SentRepresentingName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.ConversationTopic:
                        ConversationTopic = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.MessageClass:
                        MessageClass = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.SenderAddress:
                        SenderAddress = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.SenderAddressType:
                        SenderAddressType = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.SenderName:
                        SenderName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.MessageDeliveryTime:
                        MessageDeliveryTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.MessageFlags:
                        MessageFlags = BitConverter.ToUInt32(prop.Value.Data, 0);

                        Read = (MessageFlags & 0x1) != 0;
                        Unsent = (MessageFlags & 0x8) != 0;
                        Unmodified = (MessageFlags & 0x2) != 0;
                        HasAttachments = (MessageFlags & 0x10) != 0;
                        FromMe = (MessageFlags & 0x20) != 0;
                        IsFAI = (MessageFlags & 0x40) != 0;
                        NotifyReadRequested = (MessageFlags & 0x100) != 0;
                        NotifyUnreadRequested = (MessageFlags & 0x200) != 0;
                        EverRead = (MessageFlags & 0x400) != 0;
                        break;
                    case MessageProperty.MessageSize:
                        MessageSize = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case MessageProperty.InternetArticleNumber:
                        InternetArticleNumber = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case (MessageProperty)0xe27:
                        //unknown
                        break;
                    case MessageProperty.NextSentAccount:
                        //nextSentAccount, ignore this, string
                        break;
                    case (MessageProperty)0xe62:
                        //unknown
                        break;
                    case MessageProperty.TrustedSender:
                        //trusted sender
                        break;
                    case MessageProperty.Headers:
                        Headers = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.BodyPlainText:
                        BodyPlainText = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.BodyCompressedRTF:
                        BodyCompressedRTFString = new RtfDecompressor().Decompress(prop.Value.Data);
                        break;
                    case MessageProperty.BodyHtml:
                        //var temp = MessagePropertyTypes.PropertyToString(true, prop.Value);
                        BodyHtml = Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.MessageID:
                        InternetMessageID = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.UrlCompositeName:
                        UrlCompositeName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.AttributeHidden:
                        AttributeHidden = prop.Value.Data[0] == 0x01;
                        break;
                    case (MessageProperty)0x10F5:
                        //unknown
                        break;
                    case MessageProperty.ReadOnly:
                        ReadOnly = prop.Value.Data[0] == 0x01;
                        break;
                    case MessageProperty.CreationTime:
                        CreationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.LastModificationTime:
                        LastModificationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.SearchKey:
                        //seach key
                        break;
                    case MessageProperty.CodePage:
                        CodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case MessageProperty.LocaleID:
                        //localeID
                        break;
                    case MessageProperty.CreatorName:
                        CreatorName = Encoding.Unicode.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.CreatorEntryID:
                        //creator entryid
                        break;
                    case MessageProperty.LastModifierName:
                        //last modifier name
                        break;
                    case MessageProperty.LastModifierEntryID:
                        //last modifier entryid
                        break;
                    case MessageProperty.NonUnicodeCodePage:
                        NonUnicodeCodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case (MessageProperty)0x4019:
                        //unknown
                        break;
                    case MessageProperty.SentRepresentingFlags:
                        //sentrepresentingflags
                        break;
                    case MessageProperty.UserEntryID:
                        //userentryid
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
