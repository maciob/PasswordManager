using System;
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
using MahApps.Metro.Controls;
using System.Net;
using System.IO;

namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window7.xaml
    /// </summary>
    public partial class Window7 : MetroWindow
    {
        bool unsuccessfull = false;
        public Window7()
        {
            InitializeComponent();
        }

        private void Button_ImportAccount(object sender, RoutedEventArgs e)
        {
            unsuccessfull = false;
            if (!File.Exists(Username.Text + ".db"))
            {
                if (Username.Text.Length > 5)
                {
                    try
                    {
                        WebClient client = new WebClient();
                        client.DownloadFile("ftp://192.168.1.34/" + OTP.Text + ".db", Username.Text + ".db"); //download
                    }
                    catch (Exception ex)
                    {
                        unsuccessfull = true;
                        Window2 win2 = new Window2();
                        win2.Title = "Error";
                        win2.Error.Content = "Your OTP is wrong.";
                        win2.ShowDialog();
                    }
                }
                else 
                {
                    unsuccessfull = true;
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "Your username must be at least 5 characters long.";
                    win2.ShowDialog();
                }
            }
            else
            {
                unsuccessfull = true;
                Window2 win2 = new Window2();
                win2.Title = "Error";
                win2.Error.Content = "This user already exist!";
                win2.ShowDialog();
            }
            if (unsuccessfull == false) 
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.1.34/" + OTP.Text + ".db");//delete
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Window2 win2 = new Window2();
                win2.Title = "Success";
                win2.Error.Content = "The database has been downloaded successfully!";
                this.Hide();
                win2.ShowDialog();
                this.Close();
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
