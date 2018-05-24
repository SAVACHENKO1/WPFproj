
using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;       //microsoft Excel 14 object in references-> COM tab

namespace JobInterview
{
    class ExcelFileProxy
    {
        private Excel.Application _xlApp = null;
        private Excel.Workbook _xlWorkbook = null;
        private Excel._Worksheet _currentSheet = null;
        private Excel.Range _xlRange = null;
        private String _path;

        public ExcelFileProxy(string path) //constructor
        {
            _path = path;
        }

        //release com objects to fully kill excel process from running in the background
        //we might already released some of them which may throw exception
        //no elegant way to write this since those objects created by a thread
        //sometimes on immediate close errors occur and this in the purpose of this destructor
        ~ExcelFileProxy() //destructor
        {
            if (_xlRange != null)
                Marshal.FinalReleaseComObject(_xlRange);
            if (_currentSheet != null)
                Marshal.ReleaseComObject(_currentSheet);
            if (_xlWorkbook != null)
                Marshal.ReleaseComObject(_xlWorkbook);
            if (_xlApp != null)
                Marshal.ReleaseComObject(_xlApp);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Console.WriteLine("close call ");
        }

        public void ReadExcel(string conn_string)
        {
            _xlApp = new Excel.Application();
            try
            {
                _xlWorkbook = _xlApp.Workbooks.Open(_path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Invalid file name entered");
                _xlApp.Quit();
                Marshal.ReleaseComObject(_xlApp);
                return;
            }

            int totalSheetsNum = _xlWorkbook.Sheets.Count;


            //error killing com objects with foreach
            for (int currentSheetNum = 1; currentSheetNum <= totalSheetsNum; currentSheetNum++)
            {
                //just progress bar update values in range 1 to 100 integer
                // MainWindow.ChangePB(100 * (++currentSheetNum) / totalSheetsNum);

                _currentSheet = _xlWorkbook.Sheets[currentSheetNum];
                _xlRange = _currentSheet.UsedRange;

                int colCount = _xlRange.Columns.Count;
                int rowCount = _xlRange.Rows.Count;
                
                //send to the server
                DB_Handler.ConnectAndInsertConnectAndInsert(ref _xlRange, conn_string, _currentSheet.Name);

                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(_xlRange);
                Marshal.ReleaseComObject(_currentSheet);

            }
            _xlWorkbook.Close();
            Marshal.ReleaseComObject(_xlWorkbook);

            _xlApp.Quit();
            Marshal.ReleaseComObject(_xlApp);
        }
    }
}
