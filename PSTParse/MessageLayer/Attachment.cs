using System;
using System.Collections.Generic;
using System.Linq;
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
        public AttachmentMethod Method { get; set; }
        public uint Size { get; set; }
        public uint RenderingPosition { get; set; }
        public string Filename { get; set; }
        public string AttachmentLongFileName { get; set; }
        public string DisplayName { get; set; }
        public uint LTPRowID { get; set; }
        public uint LTPRowVer { get; set; }
        public bool InvisibleInHTML { get; set; }
        public bool InvisibleInRTF { get; set; }
        public bool RenderedInBody { get; set; }
        public byte[] Data { get; set; }

        public Attachment(PropertyContext propertyContext) : this(propertyContext?.Properties.Select(d => d.Value)) { }

        public Attachment(IEnumerable<ExchangeProperty> exchangeProperties)
        {
            exchangeProperties = exchangeProperties ?? Enumerable.Empty<ExchangeProperty>();
            foreach (var property in exchangeProperties)
            {
                switch (property.ID)
                {
                    case MessageProperty.AttachmentData:
                        Data = property.Data;
                        break;
                    case MessageProperty.AttachmentSize:
                        Size = BitConverter.ToUInt32(property.Data, 0);
                        break;
                    case MessageProperty.AttachmentFileName:
                        if (property.Data != null)
                            Filename = Encoding.Unicode.GetString(property.Data);
                        //else
                        //    Filename = Guid.NewGuid().ToString();
                        break;
                    case MessageProperty.DisplayName:
                        if (property.Data != null)
                            DisplayName = Encoding.Unicode.GetString(property.Data);
                        else
                            DisplayName = Guid.NewGuid().ToString();
                        break;
                    case MessageProperty.AttachmentLongFileName:
                        if (property.Data != null)
                            AttachmentLongFileName = Encoding.Unicode.GetString(property.Data);
                        //else
                        //    AttachmentLongFileName = Guid.NewGuid().ToString();
                        break;
                    case MessageProperty.AttachmentMethod:
                        Method = (AttachmentMethod)BitConverter.ToUInt32(property.Data, 0);
                        break;
                    case MessageProperty.AttachmentRenderPosition:
                        RenderingPosition = BitConverter.ToUInt32(property.Data, 0);
                        break;
                    case MessageProperty.AttachmentFlags:
                        var flags = BitConverter.ToUInt32(property.Data, 0);
                        InvisibleInHTML = (flags & 0x1) != 0;
                        InvisibleInRTF = (flags & 0x02) != 0;
                        RenderedInBody = (flags & 0x04) != 0;
                        break;
                    case MessageProperty.AttachmentLTPRowID:
                        LTPRowID = BitConverter.ToUInt32(property.Data, 0);
                        break;
                    case MessageProperty.AttachmentLTPRowVer:
                        LTPRowVer = BitConverter.ToUInt32(property.Data, 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}