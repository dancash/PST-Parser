using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse
{
    public class PSTFile
    {
        public static PSTFile CurPST { get; set; }
        public string Path { get; set; }
        public static MemoryMappedFile PSTMMF { get; set; }
        public PSTHeader Header { get; set; }

        public PSTFile(string path)
        {
            PSTFile.CurPST = this;
            this.Path = path;
            PSTFile.PSTMMF = MemoryMappedFile.CreateFromFile(path, FileMode.Open);

            this.Header = new PSTHeader();

            /*var messageStoreData = BlockBO.GetNodeData(SpecialNIDs.NID_MESSAGE_STORE);
            var temp = BlockBO.GetNodeData(SpecialNIDs.NID_ROOT_FOLDER);*/

            var pc = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE);
            //PasswordReset.ResetPassword();

        }

        public void CloseMMF()
        {
            PSTMMF.Dispose();
        }

        public void OpenMMF()
        {
            PSTMMF = MemoryMappedFile.CreateFromFile(this.Path, FileMode.Open);
        }

        public Tuple<ulong,ulong> GetNodeBIDs(ulong NID)
        {
            return this.Header.NodeBT.Root.GetNIDBID(NID);
        }

        public BBTENTRY GetBlockBBTEntry(ulong BID)
        {
            return this.Header.BlockBT.Root.GetBIDBBTEntry(BID);
        }
    }
}
