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
    /// Logika interakcji dla klasy Window4.xaml
    /// </summary>
    public partial class Window4 : MetroWindow
    {

        public bool succesfull = false;

        public Window4()
        {
            InitializeComponent();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            succesfull = true;
            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            succesfull = false;
            this.Close();
        }

        private void Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9);
        }
    }
}
