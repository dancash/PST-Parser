//using PSTParse.ListsTablesPropertiesLayer;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PSTParse.MessageLayer
//{
//    public class MessagePropertyTypes
//    {
//        public static string PropertyToString(bool unicode, ExchangeProperty prop, bool enforceMaxLength = false)
//        {
//            int maxStringBytes = enforceMaxLength ? 2048 : Int32.MaxValue;
//            try
//            {
//                if (prop.Type == ExchangeProperty.PropType.Binary && prop.Data.Length > 0)
//                {
//                    return Encoding.ASCII.GetString(prop.Data, 0, prop.Data.Length);
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Boolean && prop.Data.Length > 0)
//                {
//                    // since it's little-endian, we can just take the value of the first byte,
//                    // regardless of the total width of the value.
//                    return (prop.Data[0] != 0).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Currency)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.ErrorCode)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Floating32 && prop.Data.Length >= 4)
//                {
//                    return BitConverter.ToSingle(prop.Data, 0).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Floating64 && prop.Data.Length >= 8)
//                {
//                    return BitConverter.ToDouble(prop.Data, 0).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.FloatingTime && prop.Data.Length >= 8)
//                {
//                    return DateTime.FromBinary(BitConverter.ToInt64(prop.Data, 0)).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Guid && prop.Data.Length >= 16)
//                {
//                    return (new Guid(prop.Data)).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Integer16 && prop.Data.Length >= 2)
//                {
//                    return BitConverter.ToUInt16(prop.Data, 0).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Integer32 && prop.Data.Length >= 4)
//                {
//                    return BitConverter.ToUInt32(prop.Data, 0).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Integer64 && prop.Data.Length >= 8)
//                {
//                    return BitConverter.ToUInt64(prop.Data, 0).ToString();
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleBinary)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleCurrency)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleFloating32)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleFloating64)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleFloatingTime)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleGuid)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleInteger16)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleInteger32)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleInteger64)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleString && prop.Data.Length > 8)
//                {
//                    uint numStrings = BitConverter.ToUInt32(prop.Data, 0);
//                    // screw it, just render the first string, up until the end of the data.
//                    return unicode
//                        ? Encoding.Unicode.GetString(prop.Data, 8, Math.Min(maxStringBytes, prop.Data.Length - 8))
//                        : Encoding.ASCII.GetString(prop.Data, 8, Math.Min(maxStringBytes, prop.Data.Length - 8));
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleString8)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.MultipleTime)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Restriction)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.RuleAction)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.ServerId)
//                {
//                }
//                else if (prop.Type == ExchangeProperty.PropType.String)
//                {
//                    return unicode
//                        ? Encoding.Unicode.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length))
//                        : Encoding.ASCII.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length));
//                }
//                else if (prop.Type == ExchangeProperty.PropType.String8)
//                {
//                    return Encoding.ASCII.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length));
//                }
//                else if (prop.Type == ExchangeProperty.PropType.Time && prop.Data.Length >= 8)
//                {
//                    return DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Data, 0)).ToString();
//                }

//                // If we fall through to here, then just try to render it as ascii...
//                Encoding.ASCII.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length));

//            }
//            catch { }
//            return "";
//        }
//    }
//}
