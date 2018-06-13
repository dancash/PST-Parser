using System;
using System.Linq;
using System.Text;
using PSTParse.ListsTablesPropertiesLayer;

namespace PSTParse.MessageLayer
{
    public class Recipient
    {
        public enum RecipientType
        {
            FROM = 0x00,
            TO = 0x01,
            CC = 0x02,
            BCC = 0x03
        }

        public RecipientType Type;
        public PSTEnums.ObjectType ObjType;
        public bool Responsibility;
        public byte[] Tag;
        public EntryID EntryID;
        public string DisplayName;
        public string EmailAddress;
        public string EmailAddressType;

        public Recipient(TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                var data = exProp.Data ?? new byte[0];
                switch (exProp.ID)
                {
                    case MessageProperty.RecipientType:
                        Type = (RecipientType)BitConverter.ToUInt32(data, 0);
                        break;
                    case MessageProperty.RecipientResponsibility:
                        Responsibility = data.Any() ? data[0] == 0x01 : false;
                        break;
                    case MessageProperty.RecordKey:
                        Tag = data;
                        break;
                    case MessageProperty.RecipientObjType:
                        ObjType = (PSTEnums.ObjectType)BitConverter.ToUInt32(data, 0);
                        break;
                    case MessageProperty.RecipientEntryID:
                        EntryID = new EntryID(data);
                        break;
                    case MessageProperty.DisplayName:
                        DisplayName = Encoding.Unicode.GetString(data);
                        break;
                    case MessageProperty.AddressType:
                        EmailAddressType = Encoding.Unicode.GetString(data);
                        break;
                    case MessageProperty.AddressName:
                        EmailAddress = Encoding.Unicode.GetString(data);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}