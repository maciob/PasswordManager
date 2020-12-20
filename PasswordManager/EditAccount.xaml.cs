using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SQLite;

namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window8.xaml
    /// </summary>
    public partial class Window8 : MetroWindow
    {
        string login;
        public bool succesfull = false;
        public bool LoginChanged = false;
        public bool PasswordChanged = false;
        public bool EmailChanged = false;
        string email;
        bool error = false;
        PasswordBox pass;

        public Window8(string user, PasswordBox password, string Email)
        {
            InitializeComponent();
            if (user.Contains("."))
            {
                user = user.Remove(user.IndexOf("."), user.Length - user.IndexOf("."));
            }
            LoginLabel.Content = user;
            PasswordLabel.Content = "********";
            EmailLabel.Content = Email;
            login = user;
            pass = password;
            email = Email;
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            if (LoginText.Visibility == Visibility.Visible) 
            {
                if (LoginText.Text != login && LoginText.Text.Length >= 5)
                {
                    LoginChanged = true;
                    succesfull = true;
                }
                else 
                {
                    error = true;
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "Your login must be different than original\nand have at least 5 characters";
                    win2.ShowDialog();
                }
            }

            if (EmailText.Visibility == Visibility.Visible)
            {
                    if (EmailText.Text != email)
                    {
                        EmailChanged = true;
                        succesfull = true;
                    }
                    else
                    {
                        error = true;
                        Window2 win2 = new Window2();
                        win2.Title = "Error";
                        win2.Error.Content = "Your email must be different than original";
                        win2.ShowDialog();
                    }
            }


            //Checking if new password is legit
            if (PasswordBox.Visibility == Visibility.Visible || PasswordText.Visibility == Visibility.Visible)
            {
                if (PasswordText.Visibility != Visibility.Hidden)
                {
                    PasswordBox.Password = PasswordText.Text;
                    PasswordBox.Visibility = Visibility.Visible;
                    PasswordText.Visibility = Visibility.Hidden;
                }

                Window5 win5 = new Window5(0, 0, 0, 0, 0, false);
                if (win5.Check(PasswordBox.Password.ToString(), 1, 1, 1, 1) == true && PasswordBox.Password.Length >= 8)
                {
                    PasswordChanged = true;
                    succesfull = true;
                }
                else
                {
                    error = true;
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "Your password must consist: At least 8 characters and at least\none number, huge letter, small letter and special character.";
                    win2.ShowDialog();
                }
                win5.Close();
            }

            if (PasswordLabel.Visibility==Visibility.Visible && LoginLabel.Visibility == Visibility.Visible && EmailLabel.Visibility == Visibility.Visible) 
            {
                Window2 win2 = new Window2();
                win2.Title = "Error";
                win2.Error.Content = "Your did not changed anything.";
                win2.ShowDialog();
                error = true;
            }

            if ((PasswordChanged == true || LoginChanged == true || EmailChanged == true) && error == false)
            {
                this.Close();
            }
            else 
            {
                error = false;
                LoginChanged = false;
                EmailChanged = false;
                PasswordChanged = false;
                succesfull = false;
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_EditLogin(object sender, RoutedEventArgs e)
        {
            if (LoginText.Visibility == Visibility.Visible)
            {
                LoginText.Visibility = Visibility.Hidden;
                LoginLabel.Visibility = Visibility.Visible;
                LoginChanged = false;
            }
            else
            {
                LoginText.Visibility = Visibility.Visible;
                LoginLabel.Visibility = Visibility.Hidden;
                LoginText.Text = LoginLabel.Content.ToString();
            }
        }

        private void Button_EditPassword(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible || PasswordText.Visibility == Visibility.Visible)
            {
                PasswordBox.Visibility = Visibility.Hidden;
                PasswordText.Visibility = Visibility.Hidden;
                PasswordLabel.Visibility = Visibility.Visible;
                PasswordChanged = false;
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordLabel.Visibility = Visibility.Hidden;
                PasswordText.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Show(object sender, RoutedEventArgs e)
        {
            if (PasswordLabel.Visibility == Visibility.Hidden)
            {
                if (PasswordText.Visibility == Visibility.Hidden)
                {
                    PasswordText.Text = PasswordBox.Password.ToString();
                    PasswordText.Visibility = Visibility.Visible;
                    PasswordBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    PasswordBox.Password = PasswordText.Text;
                    PasswordBox.Visibility = Visibility.Visible;
                    PasswordText.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Button_EditEmail(object sender, RoutedEventArgs e)
        {
                if (EmailText.Visibility == Visibility.Visible)
                {
                    EmailText.Visibility = Visibility.Hidden;
                    EmailLabel.Visibility = Visibility.Visible;
                    EmailChanged = false;
                }
                else
                {
                    EmailText.Visibility = Visibility.Visible;
                    EmailLabel.Visibility = Visibility.Hidden;
                    EmailText.Text = EmailLabel.Content.ToString();
                }
        }
    }
}
