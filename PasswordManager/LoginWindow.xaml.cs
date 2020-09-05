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
using System.Diagnostics;
using SQLite;
using MahApps.Metro.Controls;
using System.Windows.Threading;

namespace PasswordManager
{

    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        ~MainWindow()
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

        private int loginTimeFail = 0;
        int time_ammount = 60;
        TimeSpan time;
        DispatcherTimer timer;
        bool isOnTimer = false;

        private void Button_Login(object sender, RoutedEventArgs e)
        {

            if (File.Exists("temp.db")&&FileInUse("temp.db")==false)
            {
                File.Delete("temp.db");
            }
            if (password_text.Visibility != Visibility.Collapsed)
            {
                password_box.Password = password_text.Text;
            }
            if (isOnTimer == false)
            {
                if (File.Exists(login_user.Text + ".db"))
                {
                    Process p = new Process();
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = "cmd.exe";
                    info.RedirectStandardInput = true;
                    info.RedirectStandardOutput = true;
                    info.RedirectStandardError = true;
                    info.UseShellExecute = false;
                    info.CreateNoWindow = true;
                    p.StartInfo = info;
                    p.Start();

                    if (checkForSQLInjection(login_user.Text) == false && checkForSQLInjection(password_box.Password.ToString()) == false)
                    {
                        using (StreamWriter sw = p.StandardInput)
                        {
                            if (sw.BaseStream.CanWrite)
                            {
                                sw.WriteLine("sqlite3 {0}.db", login_user.Text);
                                sw.WriteLine("PRAGMA key = '{0}';", password_box.Password.ToString());
                                sw.WriteLine("SELECT * FROM Website;");
                            }
                        }
                    }
                    else
                    {
                        loginTimeFail++;
                        Window2 win2 = new Window2();
                        win2.Title = "Error";
                        win2.Error.Content = "Are you trying out SQLInjection? Try again.";
                        win2.ShowDialog();
                    }

                    if (p.StandardError.ReadToEnd() != "")
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
                        if (File.Exists(login_user.Text + "1.db"))
                        {
                            if (File.GetLastWriteTime(login_user.Text + ".db").ToString().Equals(File.GetLastWriteTime(login_user.Text + "1.db").ToString())==false)
                            {
                                if (File.Exists(login_user.Text + "2.db"))
                                {
                                    if (File.Exists(login_user.Text + "3.db"))
                                    {
                                        if (File.Exists(login_user.Text + "4.db"))
                                        {
                                            if (File.Exists(login_user.Text + "5.db"))
                                            {
                                                File.Delete(login_user.Text + "5.db");
                                                File.Copy(login_user.Text + "4.db", login_user.Text + "5.db");
                                            }
                                            else
                                            {
                                                File.Copy(login_user.Text + "4.db", login_user.Text + "5.db");
                                            }
                                            File.Delete(login_user.Text + "4.db");
                                            File.Copy(login_user.Text + "3.db", login_user.Text + "4.db");
                                        }
                                        else
                                        {
                                            File.Copy(login_user.Text + "3.db", login_user.Text + "4.db");
                                        }
                                        File.Delete(login_user.Text + "3.db");
                                        File.Copy(login_user.Text + "2.db", login_user.Text + "3.db");
                                    }
                                    else
                                    {
                                        File.Copy(login_user.Text + "2.db", login_user.Text + "3.db");
                                    }
                                    File.Delete(login_user.Text + "2.db");
                                    File.Copy(login_user.Text + "1.db", login_user.Text + "2.db");
                                }
                                else
                                {
                                    File.Copy(login_user.Text + "1.db", login_user.Text + "2.db");
                                }
                                File.Delete(login_user.Text + "1.db");
                                File.Copy(login_user.Text + ".db", login_user.Text + "1.db");
                            }
                        }
                        else
                        {
                            File.Copy(login_user.Text + ".db", login_user.Text + "1.db");
                        }


                        loginTimeFail = 0;
                        this.Visibility = Visibility.Hidden;
                        Window1 win1 = new Window1(login_user.Text + ".db", password_box);
                        win1.ShowDialog();
                        this.login_user.Text = "";
                        this.password_box.Clear();
                        this.password_text.Text = "";
                        this.Visibility = Visibility.Visible;
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
                win2.Error.Content = "You have entered your password or account name incorrectly.\nPlease check your password and account name and try again in 1 minute.";
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
                Console.WriteLine(File.Exists(win6.Login.Text + ".db"));
                if (File.Exists(win6.Login.Text + ".db"))
                {
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "Such a user already exist!";
                    win2.ShowDialog();
                }
                else
                {
                    var db = new SQLiteConnection("temp.db");
                    db.CreateTable<Website>();

                    Process p = new Process();
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = "cmd.exe";
                    info.RedirectStandardInput = true;
                    info.UseShellExecute = false;
                    info.CreateNoWindow = true;
                    p.StartInfo = info;
                    p.Start();

                    if (checkForSQLInjection(login_user.Text) == false && checkForSQLInjection(password_box.Password.ToString()) == false)
                    {
                        using (StreamWriter sw = p.StandardInput)
                        {
                            if (sw.BaseStream.CanWrite)
                            {
                                sw.WriteLine("sqlite3 temp.db");
                                sw.WriteLine("ATTACH DATABASE '{0}.db' AS encrypted KEY '{1}';", win6.Login.Text, win6.Password_Box.Password.ToString());
                                sw.WriteLine("SELECT sqlcipher_export('encrypted');");
                                sw.WriteLine("DETACH DATABASE encrypted;");
                                sw.WriteLine(".quit");
                            }
                        }
                    }
                    else
                    {
                        Window2 win2 = new Window2();
                        win2.Title = "Error";
                        win2.Error.Content = "Are you trying out SQLInjection? Try again.";
                        win2.ShowDialog();
                    }
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
    }
}
