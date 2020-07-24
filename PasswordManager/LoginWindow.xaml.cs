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


namespace PasswordManager
{

    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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

        private void Button_Login(object sender, RoutedEventArgs e)
        {
            //if (File.Exists("temp.db"))
            //{
            //    File.Delete("temp.db");
            //}
            if (password_text.Visibility != System.Windows.Visibility.Collapsed)
            {
                password_box.Password = password_text.Text;
            }
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

                using (StreamWriter sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("sqlite3 " + login_user.Text + ".db");
                        sw.WriteLine("PRAGMA key = '" + password_box.Password.ToString() + "';");
                        sw.WriteLine("SELECT * FROM Website;");
                    }
                }
                if (p.StandardError.ReadToEnd() != "")
                {
                    Window2 win2 = new Window2();
                    win2.ShowDialog();
                }
                else
                {
                    this.Hide();
                    Window1 win1 = new Window1(login_user.Text + ".db", password_box);
                    win1.ShowDialog();
                    this.Close();
                }
            }
            else
            {

            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_No_Password(object sender, RoutedEventArgs e)
        {

        }

        private void Button_No_Account(object sender, RoutedEventArgs e)
        {
            Window6 win6 = new Window6();
            win6.ShowDialog();
            if (win6.succesfull == true)
            {
                Console.WriteLine(File.Exists(win6.Login.Text+".db"));
                if (File.Exists(win6.Login.Text+".db"))
                {
                    
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

                    using (StreamWriter sw = p.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            sw.WriteLine("sqlite3 temp.db");
                            sw.WriteLine("ATTACH DATABASE '" + win6.Login.Text + ".db" + "' AS encrypted KEY '" + win6.Password_Box.Password.ToString() + "';");
                            sw.WriteLine("SELECT sqlcipher_export('encrypted');");
                            sw.WriteLine("DETACH DATABASE encrypted;");
                            sw.WriteLine(".quit");
                        }
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
    }
}
