using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;       //microsoft Excel 14 object in references-> COM tab

namespace JobInterview
{
    // related enum
    public enum CellType
    {
        Blank,
        Date,
        Formula,
        Number,
        Text
    }

    class DB_Handler
    {
        private static string conn;

        public DB_Handler()
        {
        }

        public static String ConnectionStringCreator(string host, string user, string DBname, string Password, string Port)
        {
            return String.Format(
                    "Server={0}; User Id={1}; Database={2}; Port={3}; Password={4}; SSL Mode=Prefer; Trust Server Certificate=true",
                    host,
                    user,
                    DBname,
                    Port,
                    Password);
        }

        //Build connection string using parameters from portal
        public static void ConnectAndInsertConnectAndInsert(ref Excel.Range xlRange, string connString, string tableName)
        {


            List<string> colNames = new List<string>();
            for (int i = 1; i <= xlRange.Columns.Count; i++)
                colNames.Add(xlRange.Cells[1, i].Text);

            List<object> CollumnsTypes = new List<object>();
            for (int i = 1; i <= xlRange.Columns.Count; i++)
            {
                CollumnsTypes.Add(RangeCellTypeChecker.GetCellType(xlRange.Cells[3, i]));
                //Console.Write("  " + RangeCellTypeChecker.GetCellType(xlRange.Cells[2, i]));
            }

            //we will create few copies of the insert string and send them together
            for (int i = 2; i <= xlRange.Rows.Count; i++)
            {
                using (var conn = new Npgsql.NpgsqlConnection(connString))
                {
                    conn.Open();
                    List<string> rowValues = new List<string>();
                    for (int j = 1; j <= xlRange.Columns.Count; j++)
                        rowValues.Add(xlRange.Cells[i, j].Text);
                    SendStatmentExecuter(conn, colNames, rowValues, tableName, CollumnsTypes);
                    conn.Close();
                }
            }
        }

        public static void SendStatmentExecuter(Npgsql.NpgsqlConnection conn, List<string> colNames, List<string> rowValues, string tableName, List<object> CollumnsTypes)
        {
            try
            {
                int j;
                var command = conn.CreateCommand();
                var cmd = "";
                //start sql string creation
                cmd += @"INSERT INTO public." + tableName + " (";

                //enter columns names
                for (j = 1; j < colNames.Count; j++)//get column names
                    cmd += colNames[j - 1] + ", ";

                //last intersection was removed for optimization
                cmd += colNames[j - 1] + @") VALUES (";

                //enter the values except the last one
                for (j = 0; j < (colNames.Count - 1); j++)
                {
                    //if its string we need to add single brackets else we don't need to add it
                    cmd += (((CellType)CollumnsTypes[j]) is CellType.Text) ?
                        "\'" + rowValues[j] + "\', "
                        : rowValues[j] + ", ";
                    Console.WriteLine(rowValues[j]);
                }
                Console.WriteLine(rowValues[j]);
                //last tail value (difference in the end of the strings)
                cmd += (((CellType)CollumnsTypes[j]) is CellType.Text) ?
                        "\'" + rowValues[j] + "\')"
                        : rowValues[j] + " )";

                command.CommandText = cmd;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (e is ConstraintException || e is InvalidConstraintException || e is Npgsql.PostgresException)
                    MessageBox.Show("problem with input for table " + tableName );
            }

        }

    }


}


