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


namespace WpfApp4
{

    public partial class MainWindow : Window
    {

        private int Login_Times_Clicked = 0;

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
            if (File.Exists("temp.db"))
            {
                File.Delete("temp.db");
            }
            if (password_text.Visibility != System.Windows.Visibility.Collapsed)
            {
                password_box.Password = password_text.Text;
            }
            if (File.Exists(login_user.Text + ".db"))
            {
                string name = @"URI = file:";
                name += login_user.Text;
                name += ".db";
                /*string login="";
                string password="";

                var con = new SQLiteConnection(name);
                con.Open();

                string stm = "SELECT * FROM Account";

                var cmd = new SQLiteCommand(stm, con);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    login = rdr.GetString(0);
                    password = rdr.GetString(1);
                }
                if (login_user.Text == login && password_box.Password.ToString() == password)
                {
                    this.Hide();
                    Window1 win1 = new Window1(name);
                    win1.ShowDialog();
                    this.Close();
                }
                else if (Login_Times_Clicked > 2)
                {
                    Window3 win3 = new Window3();
                    win3.ShowDialog();
                }
                else
                {
                    Login_Times_Clicked++;
                    Window2 win2 = new Window2();
                    win2.ShowDialog();
                }*/
                /*Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine("sqlite3 "+ login_user.Text + ".db");
                cmd.StandardInput.WriteLine("PRAGMA key = '" + password_box.Password + "';");
                cmd.StandardInput.WriteLine("ATTACH DATABASE 'plaintext.db' AS plaintext KEY '';"); 
                cmd.StandardInput.WriteLine("SELECT sqlcipher_export('plaintext');");
                cmd.StandardInput.WriteLine("DETACH DATABASE plaintext;");
                cmd.StandardInput.WriteLine("SELECT * FROM Account;");
                cmd.StandardInput.WriteLine(".quit");

                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();

                cmd.WaitForExit();*/
                //Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                //var connection = new SQLiteConnection(login_user.Text + ".db", openFlags: SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
                //connection.Query<int>("PRAGMA key=xzy1921");
                ///path = "asdd.db"
                //var options = new SQLiteConnectionString(path,)
                //var con = new SQLiteConnection()

                this.Hide();
                Window1 win1 = new Window1(login_user.Text + ".db", password_box.Password.ToString());
                win1.ShowDialog();
                this.Close();
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
                //Console.WriteLine(File.Exists(win6.Login.Text+".db"));
                //if (File.Exists(win6.Login.Text+".db"))
                //{
                    
                //}
                //else
                {
                    var db = new SQLiteConnection("temp.db");
                    db.CreateTable<Website>();
                    /*var element = new Website
                    {
                        Website_name = "facebook",
                        Website_address = "facebook.com",
                        Login = "login",
                        Password = "haslo",
                        Date = "dzis"
                    };
                    db.Insert(element);
                    db.Insert(element);
                    db.Insert(element);
                    db.Insert(element);
                    //var conn = new SQLiteConnection(options);

                    /*cmd.CommandText = @"CREATE TABLE Account(Login TEXT,Password TEXT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO Account(Login, Password) VALUES(@Login" + ",@Password)";
                    cmd.Parameters.AddWithValue("@Login", win6.Login.Text);
                    cmd.Parameters.AddWithValue("@Password", win6.Password_Box.Password);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    */
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
