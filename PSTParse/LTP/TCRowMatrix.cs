using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class TCRowMatrix
    {
        public TableContext TableContext;
        public NodeDataDTO TCRMSubNodeData;

        public List<TCRowMatrixData> Rows;
        public Dictionary<uint, TCRowMatrixData> RowXREF; 

        public TCRowMatrix(TableContext tableContext)
        {
            this.Rows = new List<TCRowMatrixData>();
            this.RowXREF = new Dictionary<uint, TCRowMatrixData>();

            this.TableContext = tableContext;
            this.TCRMSubNodeData = this.TableContext.HeapNode.HeapSubNode[this.TableContext.TCHeader.RowMatrixLocation];
            var rowSize = this.TableContext.TCHeader.EndOffsetCEB;
            //var rowPerBlock = (8192 - 16)/rowSize;
            var dataBlocks = TCRMSubNodeData.NodeData;
            uint curIndex = 0;
            foreach(var dataBlock in dataBlocks)
            {
                for(int i = 0;i < dataBlock.Data.Length; i += rowSize)
                {
                    var curRow = new TCRowMatrixData(dataBlock.Data, this.TableContext, i);
                    this.RowXREF.Add(this.TableContext.ReverseRowIndex[curIndex], curRow);
                    this.Rows.Add(curRow);
                    curIndex++;
                }
            }
            
        }
    }
}
