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
namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window9.xaml
    /// </summary>
    public partial class Window9 : MetroWindow
    {
        public bool succesfull = false;
        public string choosenBackup= "";
        public Window9()
        {
            InitializeComponent();
            for (int i = 1; i <= 5 ; i++)
            {
                Combo.Items.Add(i);
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
