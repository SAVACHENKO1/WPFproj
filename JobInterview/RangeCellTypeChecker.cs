using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;       //microsoft Excel 14 object in references-> COM tab

//code found on https://www.reddit.com/r/dotnet/comments/3kkdjg/reading_excel_file_in_c_using_interopexcel_hhmmss/
//open source forum

namespace JobInterview
{
    class RangeCellTypeChecker
    {
        public static CellType GetCellType(Excel.Range cell)
        {
            CellType cellType;

            if (Convert.ToBoolean(cell.HasFormula))
                cellType = CellType.Formula;
            else if (cell.Value2 == null)
                cellType = CellType.Blank;
            else if (cell.Value is double || cell.Value is decimal)
                cellType = CellType.Number;
            else if (cell.Value2 is double)
                cellType = CellType.Date;
            else
                cellType = CellType.Text;

            return cellType;
        }

        // related enum
        public enum CellType
        {
            Blank,
            Date,
            Formula,
            Number,
            Text
        }
    }
}
