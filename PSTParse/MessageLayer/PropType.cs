using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.MessageLayer
{
        public enum PropType
        {
            PtypInteger16 = 0x0002,
            PtypInteger32 = 0x0003,
            PtypFloating32 = 0x0004,
            PtypFloating64 = 0x0005,
            PtypCurrency = 0x0006,
            PtypFloatingTime = 0x0007,
            PtypErrorCode = 0x000A,
            PtypBoolean = 0x000B,
            PtypInteger64 = 0x0014,
            PtypString = 0x001F,
            PtypString8 = 0x001E,
            PtypTime = 0x0040,
            PtypGuid = 0x0048,
            PtypServerId = 0x00FB,
            PtypRestriction = 0x00FD,
            PtypRuleAction = 0x00FE,
            PtypBinary = 0x0102,
            PtypMultipleInteger16 = 0x1002,
            PtypMultipleInteger32 = 0x1003,
            PtypMultipleFloating32 = 0x1004,
            PtypMultipleFloating64 = 0x1005,
            PtypMultipleCurrency = 0x1006,
            PtypMultipleFloatingTime = 0x1007,
            PtypMultipleInteger64 = 0x1014,
            PtypMultipleString = 0x101F,
            PtypMultipleString8 = 0x101E,
            PtypMultipleTime = 0x1040,
            PtypMultipleGuid = 0x1048,
            PtypMultipleBinary = 0x1102,
            PtypUnspecified = 0x0000,
            PtypNull = 0x0001,
            PtypObject = 0x000D
    }
}
