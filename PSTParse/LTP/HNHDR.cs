using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.LTP
{
    public class HNHDR
    {
        public enum ClientSig
        {
            TableContext = 0x7C,
            BTreeHeap = 0xB5,
            PropertyContext = 0xBC
        }
        public ulong OffsetHNPageMap;
        public ulong bSig;
        public ClientSig ClientSigType;
        public HID UserRoot;
        public ulong FillLevel_raw;

        public HNHDR(ref byte[] bytes)
        {
            this.ClientSigType = (ClientSig)bytes[3];
            this.bSig = bytes[2];
            this.OffsetHNPageMap = BitConverter.ToUInt16(bytes, 0);
            this.UserRoot = new HID(bytes.Skip(4).Take(4).ToArray());
            this.FillLevel_raw = BitConverter.ToUInt32(bytes, 8);
        }
    }
}
