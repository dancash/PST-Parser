using System;
using System.Collections.Generic;
using System.Text;
using PSTParse.ListsTablesPropertiesLayer;
using PSTParse.NodeDatabaseLayer;
using PSTParse.Utilities;

namespace PSTParse.MessageLayer
{
    public class Message : IPMItem
    {
        private readonly PSTFile _pst;
        private readonly ulong _nid;
        //private readonly NodeDataDTO _data;
        //private readonly TableContext _attachmentTable;
        //private readonly TableContext _recipientTable;
        //private readonly PropertyContext _attachmentPC;
        //private readonly PropertyContext _messagePC;
        private readonly Lazy<Recipients> _recipientsLazy;
        private readonly Lazy<Dictionary<ulong, NodeDataDTO>> _subNodeDataDtoLazy;
        private readonly Lazy<bool> _isRmsEncryptedLazy;
        private Lazy<List<Attachment>> _attachmentsLazy;

        public string Subject { get; set; }
        public string SubjectPrefix { get; set; }
        public Importance Imporance { get; set; }
        public Sensitivity Sensitivity { get; set; }
        public DateTime LastSaved { get; set; }
        public DateTime ClientSubmitTime { get; set; }
        public string SentRepresentingName { get; set; }
        public string ConversationTopic { get; set; }
        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderAddressType { get; set; }
        public DateTime MessageDeliveryTime { get; set; }
        public bool Read { get; set; }
        public bool Unsent { get; set; }
        public bool Unmodified { get; set; }
        public bool HasAttachments { get; set; }
        public bool FromMe { get; set; }
        public bool IsFAI { get; set; }
        public bool NotifyReadRequested { get; set; }
        public bool NotifyUnreadRequested { get; set; }
        public bool EverRead { get; set; }
        public uint MessageSize { get; set; }
        public string Headers { get; set; }
        public string BodyPlainText { get; set; }
        public string BodyHtml { get; set; }
        public uint InternetArticleNumber { get; set; }
        public string BodyCompressedRTFString { get; set; }
        public string InternetMessageID { get; set; }
        public string UrlCompositeName { get; set; }
        public bool AttributeHidden { get; set; }
        public bool ReadOnly { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public uint CodePage { get; set; }
        public String CreatorName { get; set; }
        public uint NonUnicodeCodePage { get; set; }
        public uint MessageFlags { get; set; }
        public Recipients Recipients => _recipientsLazy.Value;
        public List<Attachment> AttachmentsLazy => _attachmentsLazy.Value;
        public bool IsRMSEncrypted => _isRmsEncryptedLazy.Value;

        //public Message(PSTFile pst, ulong nid) : this(pst, new PropertyContext(nid, pst)) { }

        public Message(PSTFile pst, PropertyContext propertyContext) : base(pst, propertyContext)
        {
            _nid = propertyContext.NID;
            _pst = pst;

            _attachmentsLazy = new Lazy<List<Attachment>>(GetAttachments);
            _recipientsLazy = new Lazy<Recipients>(GetRecipients);
            _subNodeDataDtoLazy = new Lazy<Dictionary<ulong, NodeDataDTO>>(() => BlockBO.GetSubNodeData(_nid, _pst));
            _isRmsEncryptedLazy = new Lazy<bool>(GetIsRMSEncrypted);

            foreach (var prop in PropertyContext.Properties)
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

                    //already done in base
                    //case MessageProperty.MessageClass:
                    //    //MessageClassBuffer = prop.Value.Data;
                    //    MessageClass = Encoding.Unicode.GetString(prop.Value.Data);
                    //    //IsIPMNote = prop.Value.Data.Length == 16 && prop.Value.Data[14]== (byte)'e';
                    //    break;
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

        private Recipients GetRecipients()
        {
            const ulong recipientsFlag = 1682;
            var subNodeData = _subNodeDataDtoLazy.Value;
            var recipients = new Recipients();

            var exists = subNodeData.TryGetValue(recipientsFlag, out NodeDataDTO subNode);
            if (!exists) return recipients;

            var recipientTable = new TableContext(subNode);
            foreach (var row in recipientTable.RowMatrix.Rows)
            {
                var recipient = new Recipient(row);
                switch (recipient.Type)
                {
                    case Recipient.RecipientType.TO:
                        recipients.To.Add(recipient);
                        break;
                    case Recipient.RecipientType.CC:
                        recipients.CC.Add(recipient);
                        break;
                    case Recipient.RecipientType.BCC:
                        recipients.BCC.Add(recipient);
                        break;
                }
            }
            return recipients;
        }

        private List<Attachment> GetAttachments()
        {
            var attachments = new List<Attachment>();
            if (!HasAttachments) return attachments;

            var data = BlockBO.GetNodeData(_nid, _pst);
            var subNodeData = _subNodeDataDtoLazy.Value;
            var attachmentSet = new HashSet<string>();

            foreach (var subNode in subNodeData)
            {
                var nodeType = NID.GetNodeType(subNode.Key);
                //if (nodeType == NID.NodeType.ATTACHMENT_TABLE)
                //{
                //        var attachmentTable = new TableContext(subNode.Value);
                //        break;
                //}
                if (nodeType != NID.NodeType.ATTACHMENT_PC) continue;

                var attachmentPC = new PropertyContext(subNode.Value);
                var attachment = new Attachment(attachmentPC);
                if (attachmentSet.Contains(attachment.AttachmentLongFileName))
                {
                    var smallGuid = Guid.NewGuid().ToString().Substring(0, 5);
                    attachment.AttachmentLongFileName = $"{smallGuid}-{attachment.AttachmentLongFileName}";
                }
                attachmentSet.Add(attachment.AttachmentLongFileName);
                attachments.Add(attachment);
            }
            return attachments;
        }

        private bool GetIsRMSEncrypted()
        {
            if (!HasAttachments) return false;

            foreach (var attachment in AttachmentsLazy)
            {
                if (attachment.AttachmentLongFileName.ToLowerInvariant().EndsWith(".rpmsg"))
                {
                    if (AttachmentsLazy.Count > 1) throw new NotSupportedException("too many attachments for rms");
                    return true;
                }
            }
            return false;
        }
    }

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
}
