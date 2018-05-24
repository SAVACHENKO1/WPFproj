using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace JobInterview
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        //default values for testing
        private string _path { get; set; }
        private string _Host { get; set; }
        private string _User { get; set; }
        private string _DBname { get; set; }
        private string _Password { get; set; }
        private string _Port { get; set; }
        private string _DatabaseName { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //minor checking
            if (_path == null || _path.Equals("") ||
                _Host == null || _Host.Equals("") ||
                _User == null || _User.Equals("") ||
                _DBname == null || _DBname.Equals("") ||
                _Password == null || _Password.Equals("") ||
                _DatabaseName == null || _DatabaseName.Equals("") ||
                _Port == null || _Port.Equals("")
                )
                MessageBox.Show("Fill all the fields first before sending data");
            else
            {
                //string conn_string = @"Server=localhost; User Id=postgres; Database=postgres; Port=5432; Password=test1234";
                //_path = @"C:\Users\Alex\Downloads\Tutorial25.xlsx";
                String conn_string = DB_Handler.ConnectionStringCreator(_Host, _User, _DatabaseName, _Password, _Port);

                ExcelFileProxy file = new ExcelFileProxy(_path);
                try
                {
                    file.ReadExcel(conn_string);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("unknown exception please check your details or server:\n" + ex.Message);
                }
            }
        }
        private void Path_TextChanged(object sender, TextChangedEventArgs e)
        {
            _path = ((TextBox)sender).Text;
        }

        private void Host_TextChanged(object sender, TextChangedEventArgs e)
        {
            _Host = ((TextBox)sender).Text;
        }

        private void DBName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _DatabaseName = ((TextBox)sender).Text;
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            _User = ((TextBox)sender).Text;
        }

        private void Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            _Port = ((TextBox)sender).Text;
        }

        private void Passwrd_TextChanged(object sender, TextChangedEventArgs e)
        {
            _Password = ((TextBox)sender).Text;
        }
    }
}

