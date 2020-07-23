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

namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window6.xaml
    /// </summary>
    public partial class Window6 : Window
    {
        public bool succesfull = false;

        public Window6()
        {
            InitializeComponent();
        }

        private void Button_Create_Account(object sender, RoutedEventArgs e)
        {
            succesfull = true;
            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            if (Password_Text.Visibility != System.Windows.Visibility.Collapsed)
            {
                Password_Box.Password = Password_Text.Text;
            }
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Password_Text.Visibility == System.Windows.Visibility.Collapsed)
            {
                Password_Text.Text = Password_Box.Password.ToString();
                Password_Text.Visibility = System.Windows.Visibility.Visible;
                Password_Box.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Password_Box.Password = Password_Text.Text;
                Password_Box.Visibility = System.Windows.Visibility.Visible;
                Password_Text.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }
}