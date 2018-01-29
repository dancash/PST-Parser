using System;
using System.Text;
using PSTParse.ListsTablesPropertiesLayer;

namespace PSTParse.MessageLayer
{
    public enum AttachmentMethod
    {
        NONE = 0x00,
        BY_VALUE= 0x01,
        BY_REFERENCE = 0X02,
        BY_REFERENCE_ONLY = 0X04,
        EMBEDDEED_MESSAGE = 0X05,
        STORAGE = 0X06

    }
    public class Attachment
    {
        public AttachmentMethod Method;
        public uint Size;
        public uint RenderingPosition;
        public string Filename;
        public uint LTPRowID;
        public uint LTPRowVer;
        public bool InvisibleInHTML;
        public bool InvisibleInRTF;
        public bool RenderedInBody;

        public Attachment(TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                switch (exProp.ID)
                {
                    case 0x0e20:
                        this.Size = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x3704:
                        if (exProp.Data != null)
                            this.Filename = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case 0x3705:
                        this.Method = (AttachmentMethod) BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x370b:
                        this.RenderingPosition = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x3714:
                        var flags = BitConverter.ToUInt32(exProp.Data, 0);
                        this.InvisibleInHTML = (flags & 0x1) != 0;
                        this.InvisibleInRTF = (flags & 0x02) != 0;
                        this.RenderedInBody = (flags & 0x04) != 0;
                        break;
                    case 0x67F2:
                        this.LTPRowID = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x67F3:
                        this.LTPRowVer = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
