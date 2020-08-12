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
using System.Threading;
using System.Diagnostics;
using SQLite;
using MahApps.Metro.Controls;
using System.Windows.Threading;

namespace PasswordManager
{

    public partial class Window1 : MetroWindow
    {

        int Length;
        int LowerFlag;
        int UpperFlag;
        int SpecialFlag;
        int NumberFlag;
        bool ClosingFlag = false;
        public string login;
        PasswordBox password;

        Stopwatch LastInput = new Stopwatch();

        public ObservableCollection <Website> Data { get; private set; } = new ObservableCollection<Website>();

        public Window1(string user, PasswordBox pass)
        {
            InitializeComponent();
            DataContext = this;
            login = user;
            password = pass;
            Read();
            Console.WriteLine(password.Password.ToString());

            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseMoveEvent, new MouseEventHandler(OnPreviewMouseMove));
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));

            LastInput.Start();

            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Interval = TimeSpan.FromMilliseconds(1);
            if (ClosingFlag == false)
            {
                timer.Tick += timer_Tick;
                timer.Start();
            }
            else 
            {
                timer.Stop();
            }
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
            info.CreateNoWindow = true;
            p2.StartInfo = info;
            p2.Start();
            return p2;
        }

        private void Read()
        {
            System.Threading.Thread.Sleep(1000);
            var con = Connect();
            using (StreamWriter sw = con.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("sqlite3 " + login);
                    sw.WriteLine("PRAGMA key = '" + password.Password.ToString() + "';");
                    sw.WriteLine("SELECT * FROM Website;");
                    sw.WriteLine(".quit");
                }
            }

            string result = con.StandardOutput.ReadToEnd();
            var Lines = result.Split('\n');
            foreach(var line in Lines)
            {
                Console.WriteLine(line);
                var s = line.Split('|');
                if (Char.IsNumber(s[0],0))
                {
                    var element = new Website
                    {
                        ID = Int32.Parse(s[0]),
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
                Add(win5.Name_Of_Website.Text.ToString(), win5.Website.Text.ToString(), win5.Login.Text.ToString(), win5.password_box.Password.ToString() , DateTime.Today.ToString("d"));
            }
            Data.Clear();
            Read();
        }

        private void Add(string Name,string WebsiteName, string Login,string new_password,string date)
        {
            var con = Connect();
            using (StreamWriter sw = con.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("sqlite3 " + login);
                    sw.WriteLine("PRAGMA key = '" + password.Password.ToString() + "';");
                    sw.WriteLine("INSERT INTO Website( Website_name , Website_address , Login , Password , Date ) VALUES('{0}','{1}','{2}','{3}','{4}');", Name, WebsiteName, Login, new_password,date);

                    sw.WriteLine(".quit");
                }
            }
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
                using (StreamWriter sw = con.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("sqlite3 " + login);
                        sw.WriteLine("PRAGMA key = '" + password.Password.ToString() + "';");
                        sw.WriteLine("UPDATE Website SET Website_name = '{0}', Website_address = '{1}', Login = '{2}', Password = '{3}', Date = '{4}' WHERE ID = {5};", win5.Name_Of_Website.Text.ToString(), win5.Website.Text.ToString(), win5.Login.Text.ToString(), win5.password_box.Password.ToString(), DateTime.Today.ToString("d"), EditedAccount.ID);
                        sw.WriteLine(".quit");
                    }
                }
                Data.Clear();
                Read();
            }
        }

        private void Delete_Data(int ID)
        {
            var con = Connect();
            using (StreamWriter sw = con.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("sqlite3 " + login);
                    sw.WriteLine("PRAGMA key = '" + password.Password.ToString() + "';");
                    sw.WriteLine("DELETE FROM Website WHERE ID = '" + ID + "';");
                    sw.WriteLine(".quit");
                }
            }
            Data.Clear();
            Read();
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            LastInput.Restart();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            LastInput.Restart();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (LastInput.Elapsed.TotalSeconds > 5)
            {
                LastInput.Stop();
                ClosingFlag = true;
                MainWindow login = new MainWindow();
                foreach (var window in Application.Current.Windows)
                {
                    (window as Window).Hide();
                }
                login.ShowDialog();
            }
        }
    }
}
