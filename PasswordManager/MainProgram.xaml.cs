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
using System.Windows.Navigation;
using System.Web.UI.WebControls;
using SharpVectors.Dom.Svg;

namespace PasswordManager
{

    public partial class Window1 : MetroWindow
    {

        int Length;
        int LowerFlag;
        int UpperFlag;
        int SpecialFlag;
        int NumberFlag;
        
        public string login;
        PasswordBox password;

        bool changedFlag = false;

        int time_ammount = 300;
        TimeSpan time;
        DispatcherTimer timer;

        public ObservableCollection<WebsiteExpanded> Data { get; private set; } = new ObservableCollection<WebsiteExpanded>();

        public Window1(string user, PasswordBox pass)
        {
            InitializeComponent();
            DataContext = this;
            Single_Account.Visibility = Visibility.Hidden;
            login = user;
            password = pass;
            Read();

            string name = login;
            if (name.Contains("."))
            {
                name = name.Remove(name.IndexOf("."), name.Length - name.IndexOf("."));
            }
            Account.Content = name;

            //Icon
            string directory = Directory.GetCurrentDirectory();
            string path = @"\brands\person.jpg";
            path = directory + path;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            Person.Source = bitmap;

            //Timer and event handlers
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseMoveEvent, new MouseEventHandler(OnPreviewMouseMove));
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewKeyDown));

            time = TimeSpan.FromSeconds(time_ammount);

            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                TimeLeft.Text = "    Time until the end of session: " + time.ToString("c");
                if (time == TimeSpan.Zero)
                {
                    timer.Stop();
                    foreach (var window in Application.Current.Windows)
                    {
                        if (!(window is MainWindow))
                            (window as Window).Close();
                    }
                }
                time = time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
        }

        ~Window1()
        {

        }

        public class WebsiteExpanded
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public BitmapImage ImageSource { get; set; }
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
            foreach (var line in Lines)
            {
                Console.WriteLine(line);
                var s = line.Split('|');
                if (Char.IsNumber(s[0], 0))
                {
                    var element = new WebsiteExpanded
                    {
                        ID = Int32.Parse(s[0]),
                        Website_name = s[1],
                        Website_address = s[2],
                        Login = s[3],
                        Password = s[4],
                        Date = s[5]
                    };
                    string path = element.Website_address;
                    path = CharStrip(path);
                    if (File.Exists(path)) 
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(path);
                        bitmap.EndInit();
                        element.ImageSource = bitmap;
                    }
                    Data.Add(element);
                }
            }
        }
        private string CharStrip(string URL) 
        {
            if (URL.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                URL = URL.Remove(0, 8);
            }
            if (URL.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                URL = URL.Remove(0, 7);
            }
            if (URL.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                URL = URL.Remove(0, 4);
            }
            if (URL.Contains("."))
            {
                URL = URL.Remove(URL.IndexOf("."), URL.Length - URL.IndexOf("."));
            }
            string directory = Directory.GetCurrentDirectory();
            URL = @"\brands\"  + URL + ".png";
            URL = directory + URL;
            Console.WriteLine(URL);
            return URL;
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
            if (Length == 0)
            {
                Length = 16;
                LowerFlag = 1;
                UpperFlag = 1;
                SpecialFlag = 1;
                NumberFlag = 1;
            }
            Window5 win5 = new Window5(Length, LowerFlag, UpperFlag, SpecialFlag, NumberFlag, true);
            win5.Title = "Generated Password";
            win5.ShowDialog();
            if (win5.succesfull==true)
            {
                Add(win5.Name_Of_Website.Text.ToString(), win5.Website.Text.ToString(), win5.Login.Text.ToString(), win5.password_box.Password.ToString(), DateTime.Today.ToString("d"));
            }
            Data.Clear();
            Read();
        }

        private void Add(string Name, string WebsiteName, string Login, string new_password, string date)
        {
            var con = Connect();
            using (StreamWriter sw = con.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("sqlite3 " + login);
                    sw.WriteLine("PRAGMA key = '" + password.Password.ToString() + "';");
                    sw.WriteLine("INSERT INTO Website( Website_name , Website_address , Login , Password , Date ) VALUES('{0}','{1}','{2}','{3}','{4}');", Name, WebsiteName, Login, new_password, date);

                    sw.WriteLine(".quit");
                }
            }
        }
        private void Edit(object sender, RoutedEventArgs e)
        {
            Window5 win5 = new Window5(0,0,0,0,0,false);
            WebsiteExpanded EditedAccount = (WebsiteExpanded)DataGrid.SelectedItem;
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
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            PasswordBox.Visibility = Visibility.Visible;
            PasswordText.Visibility = Visibility.Hidden;
            Single_Account.Visibility = Visibility.Visible;
            WebsiteExpanded EditedAccount = (WebsiteExpanded)DataGrid.SelectedItem;
            PasswordBox SinglePassword = new PasswordBox();
            SinglePassword.Password = EditedAccount.Password;
            Single_Account.Header = EditedAccount.Website_name;
            Your_Name.Content = EditedAccount.Website_name;
            Login.Content = EditedAccount.Login;
            PasswordText.Content = SinglePassword.Password;
            PasswordBox.Content = "********";

            if (EditedAccount.Website_address.Contains("://"))
            {
                hyperlink.NavigateUri = new Uri(EditedAccount.Website_address);
            }
            else
            {
                hyperlink.NavigateUri = new Uri("http://" + EditedAccount.Website_address);
            }
            URL2.Content = EditedAccount.Website_address;
            Date.Content = EditedAccount.Date;
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            WebsiteExpanded EditedAccount = (WebsiteExpanded)DataGrid.SelectedItem;
            Delete_Data(EditedAccount.ID);
            Data.Clear();
            Read();
            Single_Account.Visibility = Visibility.Hidden;
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
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            timer.Stop();
            time = TimeSpan.FromSeconds(time_ammount);
            timer.Start();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            timer.Stop();
            time = TimeSpan.FromSeconds(time_ammount);
            timer.Start();
        }

        private void Hyperlink(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordText.Visibility == System.Windows.Visibility.Hidden)
            {
                PasswordText.Visibility = System.Windows.Visibility.Visible;
                PasswordBox.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                PasswordBox.Visibility = System.Windows.Visibility.Visible;
                PasswordBox.Content = "********";
                PasswordText.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void ShareDatabase_Click(object sender, RoutedEventArgs e)
        {
            Window3 win3 = new Window3(login);
            win3.ShowDialog();
        }


        private void AccountSettings_Click(object sender, RoutedEventArgs e)
        {
            Window8 win8 = new Window8(login);
            win8.ShowDialog();
            if (win8.succesfull==true)  
            {
                if (win8.LoginChanged == true && win8.PasswordChanged == true) 
                {
                    changedFlag = false;
                    while (changedFlag == false)
                    {
                        if (File.Exists(login) && FileInUse(login) == false)
                        {
                            changedFlag = true;
                            try
                            {
                                File.Move(login, win8.LoginText.Text + ".db");
                                login = win8.LoginText.Text + ".db";
                                Account.Content = win8.LoginText.Text;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    changedFlag = false;
                    while (changedFlag == false)
                    {
                        if (File.Exists(login) && FileInUse(login) == false)
                        {
                            changedFlag = true;
                            try
                            {
                                var con = Connect();
                                using (StreamWriter sw = con.StandardInput)
                                {
                                    if (sw.BaseStream.CanWrite)
                                    {
                                        sw.WriteLine("sqlite3 " + login);
                                        sw.WriteLine("PRAGMA key = '" + password.Password.ToString() + "';");
                                        sw.WriteLine("PRAGMA rekey = '" + win8.PasswordBox.Password.ToString() + "';");
                                        sw.WriteLine(".quit");
                                    }
                                }
                                password.Password = win8.PasswordBox.Password.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                if (win8.LoginChanged == true && win8.PasswordChanged == false)
                {
                    changedFlag = false;
                    while (changedFlag == false)
                    {
                        if (File.Exists(login) && FileInUse(login) == false)
                        {
                            changedFlag = true;
                            try
                            {
                                File.Move(login, win8.LoginText.Text + ".db");
                                login = win8.LoginText.Text + ".db";
                                Account.Content = win8.LoginText.Text;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                if (win8.LoginChanged == false && win8.PasswordChanged == true)
                {
                    changedFlag = false;
                    while (changedFlag == false)
                    {
                        if (File.Exists(login) && FileInUse(login) == false)
                        {
                            Console.WriteLine(win8.PasswordBox.Password.ToString());
                            changedFlag = true;
                            try
                            {
                                var con = Connect();
                                using (StreamWriter sw = con.StandardInput)
                                {
                                    if (sw.BaseStream.CanWrite)
                                    {
                                        sw.WriteLine("sqlite3 " + login);
                                        sw.WriteLine("PRAGMA key = '" + password.Password.ToString() + "';");
                                        sw.WriteLine("PRAGMA rekey = '" + win8.PasswordBox.Password.ToString() + "';");
                                        sw.WriteLine(".quit");
                                    }
                                }
                                password.Password = win8.PasswordBox.Password.ToString();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
        }

        private void NewAccount_Click(object sender, RoutedEventArgs e)
        {
            Window5 win5 = new Window5(0,0,0,0,0,false);
            win5.Title = "New Account";
            win5.ShowDialog();
            if (win5.succesfull)
            {
                Add(win5.Name_Of_Website.Text.ToString(), win5.Website.Text.ToString(), win5.Login.Text.ToString(), win5.password_box.Password.ToString(), DateTime.Today.ToString("d"));
            }
            Data.Clear();
            Read();
        }

        private void Button_Logout(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private bool FileInUse(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    fs.Close();
                }
                return false;
            }
            catch (IOException ex)
            {
                return true;
            }
        }
    }
}
