using System;
using System.Collections.Generic;
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
                switch (exProp.ID)
                {
                    case MessageProperty.RecipientType:
                        this.Type = (RecipientType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.RecipientResponsibility:
                        this.Responsibility = exProp.Data[0] == 0x01;
                        break;
                    case MessageProperty.RecordKey:
                        this.Tag = exProp.Data;
                        break;
                    case MessageProperty.RecipientObjType:
                        this.ObjType = (PSTEnums.ObjectType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.RecipientEntryID:
                        this.EntryID = new EntryID(exProp.Data);
                        break;
                    case MessageProperty.DisplayName:
                        this.DisplayName = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case MessageProperty.AddressType:
                        this.EmailAddressType = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    case MessageProperty.AddressName:
                        this.EmailAddress = Encoding.Unicode.GetString(exProp.Data);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
