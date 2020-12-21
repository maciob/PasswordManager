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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Diagnostics;
using SQLite;
using MahApps.Metro.Controls;
using System.Windows.Threading;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;

namespace PasswordManager
{

    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            
            tp = new GoogleTOTP();
            timer1 = new DispatcherTimer();
            timer1.Start();
            timer1.Tick += new EventHandler(timer1_Tick);
        }

        ~MainWindow()
        {

        }
        
        private int loginTimeFail = 0;
        int time_ammount = 60;
        TimeSpan time;
        DispatcherTimer timer;
        DispatcherTimer timer1;
        bool isOnTimer = false;
        GoogleTOTP tp;
        private long lastInterval;
        int maxBackup = 0;

        private void Button_Login(object sender, RoutedEventArgs e)
        {

            if (password_text.Visibility != Visibility.Collapsed)
            {
                password_box.Password = password_text.Text;
            }
            if (isOnTimer == false)
            {
                if (File.Exists(login_user.Text + ".db"))
                {
                    var db = new SQLiteConnection(login_user.Text + ".db");
                    var query = db.Table<DataStructures.Account>();
                    foreach (var account in query)
                    {
                        if (AES.Decrypt(account.Password, password_box.Password.ToString(), "PasswordManager") != password_box.Password.ToString())
                        {
                            if (isOnTimer == false && loginTimeFail >= 2)
                            {
                                isOnTimer = true;
                                time = TimeSpan.FromSeconds(time_ammount);
                                timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                                {
                                    if (time == TimeSpan.Zero)
                                    {
                                        timer.Stop();
                                        isOnTimer = false;
                                    }
                                    time = time.Add(TimeSpan.FromSeconds(-1));
                                }, Application.Current.Dispatcher);

                                Window2 win2 = new Window2();
                                win2.Title = "Error";
                                win2.Error.Content = "You have entered your password or account name incorrectly.\nPlease check your password and account name and try again\nin 1 minute.";
                                win2.ShowDialog();
                            }
                            else
                            {
                                Window2 win2 = new Window2();
                                win2.Title = "Error";
                                win2.Error.Content = "You have entered your password or account name incorrectly.\nPlease check your password and account name and try again.";
                                win2.ShowDialog();
                            }
                            loginTimeFail++;
                        }
                        else 
                        {
                            var query2 = db.Table<DataStructures.Backup>();

                            foreach (var version in query2)
                            {
                                if (maxBackup < version.ID)
                                {
                                     maxBackup = version.ID;
                                }
                            }
                            if (maxBackup != 0) 
                            {
                                var query3 = db.Table<DataStructures.Website>().Where(v => v.BackupID.Equals(maxBackup));

                                foreach(var website in query3)
                                {
                                    website.BackupID = maxBackup + 1;
                                    db.Insert(website);
                                }
                                var backup = new DataStructures.Backup
                                {
                                    ID = maxBackup + 1,
                                    Date = DateTime.Now.ToString()
                                };
                                db.Insert(backup);
                            }
                            var query4 = db.Table<DataStructures.Account>();
                            foreach (var account2 in query4) 
                            {
                                if (AES.Decrypt(account2.Email, password_box.Password.ToString(), "PasswordManager") != "") 
                                {
                                    Window11 win11 = new Window11();
                                    win11.Mode.Content = "Enter OTP sent to your email.";
                                    var message = new MimeMessage();
                                    message.From.Add(new MailboxAddress("PasswordManager", "mbekasinzynierka@gmail.com"));
                                    message.To.Add(new MailboxAddress(login_user.Text, AES.Decrypt(account2.Email, password_box.Password.ToString(), "PasswordManager")));
                                    message.Subject = "PasswordManager OTP";

                                    Window5 win5 = new Window5(10, 0, 1, 0, 1, true);
                                    string OTP = win5.Generate();
                                    message.Body = new TextPart("plain")
                                    {
                                        Text = @"Your OTP for password manager is: " + OTP
                                    };
                                    win5.Close();
                                    using (var client = new SmtpClient())
                                    {
                                        client.Connect("smtp.gmail.com", 587);
                                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                                        client.Authenticate("mbekasinzynierka@gmail.com", "PracaInzynierska12");
                                        client.Send(message);
                                        client.Disconnect(true);
                                    }
                                    win11.OTP.Text = "";
                                    win11.ShowDialog();
                                    if (OTP == win11.OTP.Text)
                                    {
                                        loginTimeFail = 0;
                                        this.Visibility = Visibility.Hidden;
                                        Window1 win1 = new Window1(login_user.Text + ".db", password_box, AES.Decrypt(account.Email, password_box.Password.ToString(), "PasswordManager"));
                                        db.Close();
                                        win1.ShowDialog();
                                        this.login_user.Text = "";
                                        this.password_box.Clear();
                                        this.password_text.Text = "";
                                        this.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        Window2 win2 = new Window2();
                                        db.Close();
                                        win2.Title = "Error";
                                        win2.Error.Content = "You have entered your OTP incorrectly.\nPlease check your OTP and try again.";
                                        win2.ShowDialog();
                                    }
                                }
                                else if(AES.Decrypt(account2.Code, password_box.Password.ToString(), "PasswordManager") != "") 
                                {
                                    Window11 win11 = new Window11();
                                    byte [] b = new byte[10];
                                    string[] subs = AES.Decrypt(account2.Code, password_box.Password.ToString(), "PasswordManager").Split(' ');
                                    int i = 0;
                                    foreach (var sub in subs)
                                    {
                                        if (i == 10) break;
                                        b[i] = Convert.ToByte(Convert.ToInt32(sub));
                                        i++;
                                    }
                                    win11.Mode.Content = "Enter OTP generated in Google Authenticator app.";
                                    win11.OTP.Text = "";
                                    win11.ShowDialog();
                                    if (tp.generateResponseCode(tp.getCurrentInterval(), b) == win11.OTP.Text)
                                    {
                                        loginTimeFail = 0;
                                        this.Visibility = Visibility.Hidden;
                                        Window1 win1 = new Window1(login_user.Text + ".db", password_box, AES.Decrypt(account.Email, password_box.Password.ToString(), "PasswordManager"));
                                        db.Close();
                                        win1.ShowDialog();
                                        this.login_user.Text = "";
                                        this.password_box.Clear();
                                        this.password_text.Text = "";
                                        this.Visibility = Visibility.Visible;
                                    }
                                    else
                                    {
                                        Window2 win2 = new Window2();
                                        db.Close();
                                        win2.Title = "Error";
                                        win2.Error.Content = "You have entered your OTP incorrectly.\nPlease check your OTP and try again.";
                                        win2.ShowDialog();
                                    }
                                }
                                if (AES.Decrypt(account2.Code, password_box.Password.ToString(), "PasswordManager") == "" && AES.Decrypt(account2.Email, password_box.Password.ToString(), "PasswordManager") == "")
                                { 
                                    loginTimeFail = 0;
                                    this.Visibility = Visibility.Hidden;
                                    Window1 win1 = new Window1(login_user.Text + ".db", password_box, AES.Decrypt(account.Email, password_box.Password.ToString(), "PasswordManager"));
                                    db.Close();
                                    win1.ShowDialog();
                                    this.login_user.Text = "";
                                    this.password_box.Clear();
                                    this.password_text.Text = "";
                                    this.Visibility = Visibility.Visible;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "You have entered your password or account name incorrectly.\nPlease check your password and account name and try again.";
                    win2.ShowDialog();
                }
            }
            else 
            {
                Window2 win2 = new Window2();
                win2.Title = "Error";
                win2.Error.Content = "You have entered your password or account name incorrectly.\nPlease check your password and account name and try again.";
                win2.ShowDialog();
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_ImportAccount(object sender, RoutedEventArgs e)
        {
            Window7 win7 = new Window7();
            win7.ShowDialog();
        }

        private void Button_No_Account(object sender, RoutedEventArgs e)
        {
            Window6 win6 = new Window6();
            win6.ShowDialog();
            if (win6.succesfull == true)
            {
                if (File.Exists(win6.Login.Text + ".db"))
                {
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "Such a user already exist!";
                    win2.ShowDialog();
                }
                else
                {
                    
                    if (win6.Flag_TwoFA == true && win6.Flag_GoogleAuthenticator == true)
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
                    }
                    var db = new SQLiteConnection(win6.Login.Text + ".db");
                    db.CreateTable<DataStructures.Website>();
                    db.CreateTable<DataStructures.Backup>();
                    db.CreateTable<DataStructures.Account>();

                    db.Execute("SELECT Website.BackupID, Backup.ID FROM Website INNER JOIN Backup ON Website.BackupID = Backup.ID");
                    string builder = "";
                    if (win6.Flag_GoogleAuthenticator == true) 
                    {
                        foreach (byte b in tp.randomBytes)
                        {
                            builder = builder + b + " ";
                        }
                    }

                    DataStructures.Account account = new DataStructures.Account
                    {
                        Password = AES.Encrypt(win6.Password_Box.Password.ToString(), win6.Password_Box.Password.ToString(), "PasswordManager"),
                        Email = AES.Encrypt(win6.Email.Text, win6.Password_Box.Password.ToString(), "PasswordManager"),
                        Code = AES.Encrypt(builder, win6.Password_Box.Password.ToString(), "PasswordManager"),
                    };
                    DataStructures.Backup backup = new DataStructures.Backup
                    {
                        ID = 0
                    };
                    db.Insert(account);
                    db.Insert(backup);
                    db.Close();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (password_text.Visibility == System.Windows.Visibility.Collapsed)
            {
                password_text.Text = password_box.Password.ToString();
                password_text.Visibility = System.Windows.Visibility.Visible;
                password_box.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                password_box.Password = password_text.Text;
                password_box.Visibility = System.Windows.Visibility.Visible;
                password_text.Visibility = System.Windows.Visibility.Collapsed;
            }
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
