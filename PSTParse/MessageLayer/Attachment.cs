using System;
using System.Text;
using PSTParse.ListsTablesPropertiesLayer;

namespace PSTParse.MessageLayer
{
    public enum AttachmentMethod
    {
        NONE = 0x00,
        BY_VALUE = 0x01,
        BY_REFERENCE = 0X02,
        BY_REFERENCE_ONLY = 0X04,
        EMBEDDEED_MESSAGE = 0X05,
        STORAGE = 0X06

    }

    public class Attachment
    {
        public AttachmentMethod Method;
        public uint Size { get; set; }
        public uint RenderingPosition { get; set; }
        public string Filename { get; set; }
        public uint LTPRowID { get; set; }
        public uint LTPRowVer { get; set; }
        public bool InvisibleInHTML { get; set; }
        public bool InvisibleInRTF { get; set; }
        public bool RenderedInBody { get; set; }

        public Attachment(TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                switch ((MessageProperty)exProp.ID)
                {
                    case MessageProperty.AttachmentSize:
                        this.Size = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentFileName:
                        if (exProp.Data != null)
                            Filename = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case MessageProperty.AttachmentMethod:
                        Method = (AttachmentMethod)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentRenderPosition:
                        RenderingPosition = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentFlags:
                        var flags = BitConverter.ToUInt32(exProp.Data, 0);
                        InvisibleInHTML = (flags & 0x1) != 0;
                        InvisibleInRTF = (flags & 0x02) != 0;
                        RenderedInBody = (flags & 0x04) != 0;
                        break;
                    case MessageProperty.AttachmentLTPRowID:
                        LTPRowID = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentLTPRowVer:
                        LTPRowVer = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}