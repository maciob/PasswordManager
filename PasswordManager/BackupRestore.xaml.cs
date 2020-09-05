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
using System.IO;
namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window9.xaml
    /// </summary>
    public partial class Window9 : MetroWindow
    {
        public bool succesfull = false;
        public string choosenBackup= "";
        public Window9(string login)
        {
            InitializeComponent();
            if (File.Exists(login + "1.db"))
            {
                if (File.Exists(login + "2.db"))
                {
                    if (File.Exists(login + "3.db"))
                    {
                        if (File.Exists(login + "4.db"))
                        {
                            if (File.Exists(login + "5.db"))
                            {
                                Combo.Items.Add(File.GetLastWriteTime(login + "5.db").ToString());
                            }
                            Combo.Items.Add(File.GetLastWriteTime(login + "4.db").ToString());
                        }
                        Combo.Items.Add(File.GetLastWriteTime(login + "3.db").ToString());
                    }
                    Combo.Items.Add(File.GetLastWriteTime(login + "2.db").ToString());
                }
                Combo.Items.Add(File.GetLastWriteTime(login + "1.db").ToString());
            }
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            if (Combo.SelectedItem != null)
            {
                succesfull = true;
                choosenBackup = Combo.SelectedItem.ToString();
                this.Close();
            }
            else 
            {
                Window2 win2 = new Window2();
                win2.Title = "Error";
                win2.Error.Content = "You must choose one of the backups.";
                win2.ShowDialog();
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
