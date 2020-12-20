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
    /// Logika interakcji dla klasy Window11.xaml
    /// </summary>
    public partial class Window11 : MetroWindow
    {
        
        public Window11()
        {
            InitializeComponent();
        }

        public bool succesfull = false;

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            succesfull = true;
            this.Close();
        }
    }
}
