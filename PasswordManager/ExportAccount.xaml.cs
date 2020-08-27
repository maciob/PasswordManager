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
using System.Windows.Threading;
using MahApps.Metro.Controls;
using System.Net;
using System.IO;

namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window3.xaml
    /// </summary>
    public partial class Window3 : MetroWindow
    {
        string Login;
        TimeSpan time;
        DispatcherTimer timer;
        int time_ammount = 60;
        bool deleteFlag = false;
        public Window3(string login)
        {
            InitializeComponent();
            Login = login;
        }
        ~Window3()
        {
            Cancel();
        }
        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Cancel();
            this.Close();
        }

        private void Cancel() 
        {
            timer.Stop();
            DeleteFile(OTP.Text + ".db");
        }


        private void Button_GenerateOTP(object sender, RoutedEventArgs e)
        {
            if (OTP.Text != "") 
            {
                DeleteFile(OTP + ".db");
            }
            Window5 win5 = new Window5(10, 0, 1, 0, 1, true);
            OTP.Text = win5.Generate();
            win5.Close();
            File.Copy(Login, OTP.Text + ".db");

            try
            {
                WebClient client = new WebClient();
                client.UploadFile("ftp://192.168.1.34/" + OTP.Text + ".db", OTP.Text + ".db"); //upload
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            time = TimeSpan.FromSeconds(time_ammount);

            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                TimeLeft.Text = time.ToString("c");
                if (time == TimeSpan.Zero)
                {
                    timer.Stop();
                    TimeLeft.Text = "OTP has expired.";
                    DeleteFile(OTP.Text+ ".db");
                }
                time = time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
        }
        private void DeleteFile(string OTP) 
        {
            while (deleteFlag == false)
            {
                if (File.Exists(OTP) && FileInUse(OTP)==false)
                {
                    deleteFlag = true;
                    try
                    {
                        File.Delete(OTP);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

            deleteFlag = false;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.1.34/" + OTP);//delete
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
        private bool FileInUse(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
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
