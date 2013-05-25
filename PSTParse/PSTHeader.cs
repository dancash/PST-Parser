using System;
using System.Text;
using PSTParse.NDB;

namespace PSTParse
{
    public class PSTHeader
    {
        public string DWMagic { get; set; }
        public bool? IsANSI { get; set; }

        public bool? IsUNICODE { get { return IsANSI == null ? null : !IsANSI; } }

        public NDB.PSTBTree NodeBT { get; set; }
        public NDB.PSTBTree BlockBT { get; set; }

        public BlockEncoding EncodingAlgotihm;
        public enum BlockEncoding
        {
            NONE=0,
            PERMUTE=1,
            CYCLIC=2
        }

        public PSTHeader(PSTFile pst)
        {
            using(var mmfView = pst.PSTMMF.CreateViewAccessor(0, 684))
            {
                var temp = new byte[4];
                mmfView.ReadArray(0, temp, 0, 4);
                this.DWMagic = Encoding.Default.GetString(temp);

                var ver = mmfView.ReadInt16(10);


                this.IsANSI = ver == 14 || ver == 15 ? true : (ver == 23 ? (bool?)false : null);

                if (this.IsANSI != null && this.IsANSI.Value)
                {
                    
                } else if (this.IsUNICODE != null && this.IsUNICODE.Value)
                {
                    //root.PSTSize = ByteReverse.ReverseULong(root.PSTSize);
                    var sentinel = mmfView.ReadByte(512);
                    var cryptMethod = (uint) mmfView.ReadByte(513);

                    this.EncodingAlgotihm = (BlockEncoding) cryptMethod;

                    var bytes = new byte[16];
                    mmfView.ReadArray(216, bytes, 0, 16);
                    var nbt_bref = new BREF(bytes);

                    mmfView.ReadArray(232, bytes, 0, 16);
                    var bbt_bref = new BREF(bytes);

                    this.NodeBT = new NDB.PSTBTree(nbt_bref, pst);
                    this.BlockBT = new NDB.PSTBTree(bbt_bref, pst);
                }
            }
        }
    }
}
