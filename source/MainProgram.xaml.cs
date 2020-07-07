using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using SQLite;


namespace WpfApp4
{

    public partial class Window1 : Window
    {

        int Length;
        int LowerFlag;
        int UpperFlag;
        int SpecialFlag;
        int NumberFlag;

        public string login;
        public string password;

        public ObservableCollection <Website> Data { get; private set; } = new ObservableCollection<Website>();

        public Window1(string file, string pass)
        {
            InitializeComponent();
            DataContext = this;
            login = file;
            password = pass;
            Read();
        }

        ~Window1()
        {

        }

        public class Website
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string Website_name { get; set; }
            public string Website_address { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string Date { get; set; }
        }

        Process Connect()
        {
            Process p2 = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            p2.StartInfo = info;
            p2.Start();
            return p2;
        }

        private void Read()
        {
            var con = Connect();
            using (StreamWriter sw = con.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("sqlite3 " + login);
                    sw.WriteLine("PRAGMA key = '" + password + "';");
                    sw.WriteLine("SELECT * FROM Website;");
                }
            }

            string result = con.StandardOutput.ReadToEnd();
            var Lines = result.Split('\n');
            Console.WriteLine(Lines);
            foreach(var line in Lines)
            {
                Console.WriteLine(line);
                var s = line.Split('|');
                if (Char.IsNumber(s[0],0))
                {
                    var element = new Website
                    {
                        Website_name = s[1],
                        Website_address = s[2],
                        Login = s[3],
                        Password = s[4],
                        Date = s[5]
                    };
                    Data.Add(element);
                }
            }
        }

        private void Button_Gen_Set(object sender, RoutedEventArgs e)
        {
            Window4 win4 = new Window4();
            win4.ShowDialog();
            if (win4.succesfull == true)
            {
                Length = int.Parse(win4.Num_char.Text);
                if (win4.Flag_lower.IsChecked == true)
                    LowerFlag = 1;
                else
                    LowerFlag = 0;
                if (win4.Flag_upper.IsChecked == true)
                    UpperFlag = 1;
                else
                    UpperFlag = 0;
                if (win4.Flag_spec.IsChecked == true)
                    SpecialFlag = 1;
                else
                    SpecialFlag = 0;
                if (win4.Flag_num.IsChecked == true)
                    NumberFlag = 1;
                else
                    NumberFlag = 0;
            }
        }

        private void Button_Generate(object sender, RoutedEventArgs e)
        {
            Window5 win5 = new Window5();
            win5.Generate(Length, LowerFlag, UpperFlag, SpecialFlag, NumberFlag);
            win5.ShowDialog();
            if (win5.succesfull)
            {
                Add(win5.Name_Of_Website.Text, win5.Website.Text, win5.Login.Text, win5.password_box.Password, DateTime.Today.ToString("d"));
                Data.Clear();
                Read();
            }
        }

        private void Add(string Name,string WebsiteName, string Login,string password,string date)
        {
            var con = Connect();
            var Account = new Website()
            {
                Website_name = Name,
                Website_address = WebsiteName,
                Login = Login,
                Password = password,
                Date = date
            };
            //con.Insert(Account);
            Data.Clear();
            Read();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Window5 win5 = new Window5();
            Website EditedAccount = (Website)DataGrid.SelectedItem;
            win5.Title = "Edit Account";
            win5.Name_Of_Website.Text = EditedAccount.Website_name;
            win5.Website.Text = EditedAccount.Website_address;
            win5.Login.Text = EditedAccount.Login;
            win5.password_box.Password = EditedAccount.Password;
            win5.ShowDialog();
            if (win5.succesfull == true)
            {
                var con = Connect();
  
                /*cmd.CommandText = "UPDATE Website SET Name = @Name, Website_Name = @Website_Name, Login = @Login, Password=@Password, Date = @Date WHERE ID = @ID";
                cmd.Parameters.AddWithValue("@Name", win5.Name_Of_Website.Text);
                cmd.Parameters.AddWithValue("@Website_Name", win5.Website.Text);
                cmd.Parameters.AddWithValue("@Login", win5.Login.Text);
                cmd.Parameters.AddWithValue("@Password", win5.password_text.Text);
                cmd.Parameters.AddWithValue("@Date", DateTime.Today.ToString("d"));
                cmd.Parameters.AddWithValue("@ID", EditedAccount.ID);
                cmd.Prepare();*/
                //cmd.ExecuteNonQuery();
                Data.Clear();
                Read();
            }

    
        }

    }
}
