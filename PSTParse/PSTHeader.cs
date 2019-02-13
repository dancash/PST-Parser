using System;
using System.Text;
using PSTParse.NodeDatabaseLayer;

namespace PSTParse
{
    public class PSTHeader
    {
        public string DWMagic { get; private set; }
        public bool IsANSI { get; }
        public bool IsUNICODE { get; }
        public NodeDatabaseLayer.PSTBTree NodeBT { get; private set; }
        public NodeDatabaseLayer.PSTBTree BlockBT { get; private set; }
        public BlockEncoding EncodingAlgotihm { get; private set; }
        public PSTRoot Root { get; }

        public PSTHeader(PSTFile pst)
        {
            using (var mmfView = pst.PSTMMF.CreateViewAccessor(0, 684))
            {
                var dwMagicBuffer = new byte[4];
                mmfView.ReadArray(0, dwMagicBuffer, 0, 4);
                DWMagic = Encoding.Default.GetString(dwMagicBuffer);

                var ver = mmfView.ReadInt16(10);


                IsANSI = ver == 14 || ver == 15;
                IsUNICODE = ver == 23;

                if (IsUNICODE)
                {
                    var fileSizeBuffer = new byte[8];
                    mmfView.ReadArray(184, fileSizeBuffer, 0, 8);
                    var fileSizeBytes = BitConverter.ToUInt64(fileSizeBuffer, 0);
                    Root = new PSTRoot(fileSizeBytes);

                    var sentinel = mmfView.ReadByte(512);
                    var cryptMethod = (uint)mmfView.ReadByte(513);

                    EncodingAlgotihm = (BlockEncoding)cryptMethod;

                    var bytes = new byte[16];
                    mmfView.ReadArray(216, bytes, 0, 16);
                    var nbt_bref = new BREF(bytes);

                    mmfView.ReadArray(232, bytes, 0, 16);
                    var bbt_bref = new BREF(bytes);

                    NodeBT = new NodeDatabaseLayer.PSTBTree(nbt_bref, pst);
                    BlockBT = new NodeDatabaseLayer.PSTBTree(bbt_bref, pst);
                }
            }
        }

        public enum BlockEncoding
        {
            NONE = 0,
            PERMUTE = 1,
            CYCLIC = 2
        }
    }
}
