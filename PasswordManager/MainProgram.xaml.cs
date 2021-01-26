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
using System.Net;

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
        public string Email;
        bool changedFlag = false;
        public int maxBackup = 0;
        int time_ammount = 300;
        TimeSpan time;
        DispatcherTimer timer;

        DispatcherTimer timer1;
        GoogleTOTP tp;
        private long lastInterval;

        public ObservableCollection<WebsiteExpanded> Data { get; set; } = new ObservableCollection<WebsiteExpanded>();

        public Window1(string user, PasswordBox pass,string email)
        {
            InitializeComponent();
            DataContext = this;
            Single_Account.Visibility = Visibility.Hidden;
            login = user;
            password = pass;
            Email = email;
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


            tp = new GoogleTOTP();
            timer1 = new DispatcherTimer();
            timer1.Start();
            timer1.Tick += new EventHandler(timer1_Tick);
        }

        ~Window1() { }

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
            public int BackupID { get; set; }
        }

        private void Read()
        {
            var db = new SQLiteConnection(login);
            var query = db.Table<DataStructures.Backup>();
            
            foreach (var version in query) 
            {
                if (maxBackup < version.ID) 
                {
                    maxBackup = version.ID;
                }
            }
            var query2 = db.Table<DataStructures.Website>().Where(v => v.BackupID.Equals(maxBackup));

            foreach (var website in query2) 
            {
                var element = new WebsiteExpanded
                {
                    ID = website.ID,
                    Website_name = AES.Decrypt(website.Website_name, password.Password.ToString(), website.Date),
                    Website_address = AES.Decrypt(website.Website_address, password.Password.ToString(), website.Date),
                    Login = AES.Decrypt(website.Login, password.Password.ToString(), website.Date),
                    Password = AES.Decrypt(website.Password, password.Password.ToString(), website.Date),
                    Date = website.Date
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
            db.Close();

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
        }

        private void Add(string Name, string WebsiteName, string Login, string new_password, string date)
        {
            var element = new DataStructures.Website
            {
                Website_name = AES.Encrypt(Name, password.Password.ToString(), date),
                Website_address = AES.Encrypt(WebsiteName, password.Password.ToString(), date),
                Login = AES.Encrypt(Login, password.Password.ToString(), date),
                Password = AES.Encrypt(new_password, password.Password.ToString(), date),
                Date = date,
                BackupID = maxBackup
            };
            var db = new SQLite.SQLiteConnection(login);
            db.Insert(element);
            Data.Clear();
            Read();
            db.Close();
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            if ((WebsiteExpanded)DataGrid.SelectedItem != null)
            {
                Window5 win5 = new Window5(0, 0, 0, 0, 0, false);
                WebsiteExpanded EditedAccount = (WebsiteExpanded)DataGrid.SelectedItem;
                win5.Title = "Edit Account";
                win5.Name_Of_Website.Text = EditedAccount.Website_name;
                win5.Website.Text = EditedAccount.Website_address;
                win5.Login.Text = EditedAccount.Login;
                win5.password_box.Password = EditedAccount.Password;
                win5.ShowDialog();
                if (win5.succesfull == true)
                {
                    var db = new SQLiteConnection(login);
                    db.Execute("UPDATE Website SET Website_name = ?, Website_address = ?, Login = ?, Password = ?, Date = ? WHERE ID = ?",
                        AES.Encrypt(win5.Name_Of_Website.Text.ToString(),password.Password.ToString(), DateTime.Today.ToString("d")),
                        AES.Encrypt(win5.Website.Text.ToString(), password.Password.ToString(), DateTime.Today.ToString("d")),
                        AES.Encrypt(win5.Login.Text.ToString(), password.Password.ToString(), DateTime.Today.ToString("d")),
                        AES.Encrypt(win5.password_box.Password.ToString(), password.Password.ToString(), DateTime.Today.ToString("d")),
                        DateTime.Today.ToString("d"),
                        EditedAccount.ID,
                        EditedAccount.BackupID
                    );

                    Data.Clear();
                    Read();
                    db.Close();
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if ((WebsiteExpanded)DataGrid.SelectedItem != null)
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordText.Visibility = Visibility.Hidden;
                Single_Account.Visibility = Visibility.Visible;
                Story.Visibility = Visibility.Hidden;
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

                string path = EditedAccount.Website_address;
                path = CharStrip(path);
                if (File.Exists(path))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path);
                    bitmap.EndInit();
                    Icon.Source = bitmap;
                }
                else
                {
                    Icon.Source = null;
                }
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if ((WebsiteExpanded)DataGrid.SelectedItem != null)
            {
                WebsiteExpanded EditedAccount = (WebsiteExpanded)DataGrid.SelectedItem;
                var db = new SQLiteConnection(login);
                db.Execute("DELETE FROM Website WHERE ID = ?", EditedAccount.ID);
                Data.Clear();
                Read();
                db.Close();
                Single_Account.Visibility = Visibility.Hidden;
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
            var db = new SQLiteConnection(login);
            var query = db.Table<DataStructures.Account>();
            bool flag2FA = false;
            bool flagGoogle = false;
            bool flagEmail = false;

            foreach (var a in query) 
            {
                if (a.Email != ""|| a.Code!= "") flag2FA = true;
                if (a.Code != "") flagGoogle = true;
                if (a.Email != "") flagEmail = true;
            }
            db.Close();
            Window8 win8 = new Window8(login, password, Email, flag2FA, flagEmail, flagGoogle);
            
            win8.ShowDialog();
            if (win8.succesfull == true)
            {
                if (win8.LoginChanged == true)
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
                System.Threading.Thread.Sleep(1000);
                changedFlag = false;

                db = new SQLiteConnection(login);
                query = db.Table<DataStructures.Account>();
                foreach (var account in query)
                {
                    if (win8.PasswordChanged == true)
                    {
                        db.Execute("UPDATE Account SET Password = ? WHERE Password = ?",
                            AES.Encrypt(win8.PasswordBox.Password.ToString(), win8.PasswordBox.Password.ToString(), "PasswordManager"),
                            account.Password
                        );

                        db.Execute("UPDATE Account SET Email = ? WHERE Email = ?",
                           AES.Encrypt(Email, win8.PasswordBox.Password.ToString(), "PasswordManager"),
                           account.Email
                        );

                        db.Execute("UPDATE Account SET Code = ? WHERE Code = ?",
                            AES.Encrypt(AES.Decrypt(account.Code,password.Password.ToString(), "PasswordManager"), win8.PasswordBox.Password.ToString(), "PasswordManager"),
                            account.Code
                        );

                        var query2 = db.Table<DataStructures.Backup>();

                        foreach (var version in query2)
                        {
                            if (version.ID == maxBackup)
                            {
                                var query3 = db.Table<DataStructures.Website>().Where(v => v.BackupID.Equals(version.ID));
                                foreach (var website in query3)
                                {
                                    website.Login = AES.Encrypt(AES.Decrypt(website.Login, password.Password.ToString(), website.Date), win8.PasswordBox.Password.ToString(), website.Date);
                                    website.Password = AES.Encrypt(AES.Decrypt(website.Password, password.Password.ToString(), website.Date), win8.PasswordBox.Password.ToString(), website.Date);
                                    website.Website_address = AES.Encrypt(AES.Decrypt(website.Website_address, password.Password.ToString(), website.Date), win8.PasswordBox.Password.ToString(), website.Date);
                                    website.Website_name = AES.Encrypt(AES.Decrypt(website.Website_name, password.Password.ToString(), website.Date), win8.PasswordBox.Password.ToString(), website.Date);
                                    website.BackupID = maxBackup + 1;
                                    db.Insert(website);
                                }
                                version.ID = maxBackup + 1;
                                version.Date = DateTime.Now.ToString();
                                db.Insert(version);
                                changedFlag = true;
                                password.Password = win8.PasswordBox.Password.ToString();
                            }
                        }
                    }
                    
                    if (win8.outEmail == true && win8.EmailChanged == true)
                    {
                        db.Execute("UPDATE Account SET Email = ? WHERE Email = ?",
                            AES.Encrypt(win8.EmailText.Text, password.Password.ToString(), "PasswordManager"),
                            account.Email
                        );
                        Email = win8.EmailText.Text;
                        changedFlag = true;
                    }

                    if (win8.outEmail == false && win8.EmailChanged == true)
                    {
                        db.Execute("UPDATE Account SET Email = ? WHERE Email = ?",
                            "",
                            account.Email
                        );
                        Email = "";
                    }

                    if (win8.outGoogle == true && win8.GoogleFAChanged == true)
                    {
                        Window10 win10 = new Window10();
                        string randomString = Transcoder.Base32Encode(tp.randomBytes);
                        
                        string ProvisionUrl = tp.UrlEncode(String.Format("otpauth://totp/{0}?secret={1}", "PasswordManager", randomString));
                        string url = String.Format("http://chart.apis.google.com/chart?cht=qr&chs={0}x{1}&chl={2}", 200, 200, ProvisionUrl);

                        WebClient wc = new WebClient();
                        var data = wc.DownloadData(url);

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(data);
                        bitmap.EndInit();
                        win10.QRcode.Source = bitmap;
                        win10.CodeLabel.Content = "Your code:\n " + tp.getPrivateKey(tp.randomBytes);
                        win10.ShowDialog();

                        string builder = "";
                        foreach (byte b in tp.randomBytes)
                        {
                            builder = builder + b + " ";
                        }

                        db.Execute("UPDATE Account SET Code = ? WHERE Code = ?",
                            AES.Encrypt(builder, password.Password.ToString(), "PasswordManager"),
                            account.Code
                        );
                        changedFlag = true;
                    }

                    if (win8.outGoogle == false && win8.GoogleFAChanged == true) 
                    {
                        db.Execute("UPDATE Account SET Code = ? WHERE Code = ?",
                            "",
                            account.Code
                        ); 
                    }
                    
                    db.Close();
                    
                    if (changedFlag == true) 
                    {
                        this.Close();
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

        private void Button_Backup(object sender, RoutedEventArgs e)
        {
            Window9 win9 = new Window9(login);
            win9.ShowDialog();
            if (win9.succesfull==true) 
            {
                var db = new SQLite.SQLiteConnection(login);
                var query = db.Table<DataStructures.Backup>();

                foreach (var version in query)
                {
                    if (version.ID==win9.choosenBackup)
                    {
                        var query2 = db.Table<DataStructures.Website>().Where(v => v.BackupID.Equals(version.ID));
                        foreach (var website in query2)
                        {
                            website.BackupID = maxBackup + 1;
                            db.Insert(website);
                        }
                        version.ID = maxBackup + 1;
                        version.Date = DateTime.Now.ToString();
                        db.Insert(version);
                        db.Close();
                        this.Close();
                    }
                }
            }
        }

        public static Boolean checkForSQLInjection(string userInput)
        {
            bool isSQLInjection = false;
            string[] sqlCheckList = { "--", ";--", ";", "/*", "*/", "@@", "@", "char", "nchar", "varchar", "nvarchar", "alter", "begin", "cast", "create", "cursor", "declare", "delete", "drop", "end", "exec", "execute", "fetch", "insert", "kill", "select", "sys", "sysobjects", "syscolumns", "table", "update" };
            string CheckString = userInput.Replace("'", "''");
            for (int i = 0; i <= sqlCheckList.Length - 1; i++)
            {
                if ((CheckString.IndexOf(sqlCheckList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    isSQLInjection = true;
                }
            }
            return isSQLInjection;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long thisInterval = tp.getCurrentInterval();
            if (lastInterval != thisInterval)
            {
                lastInterval = thisInterval;
            }
        }
    }
}
