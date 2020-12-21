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

namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window8.xaml
    /// </summary>
    public partial class Window8 : MetroWindow
    {
        public bool succesfull = false;

        string login;
        public bool LoginChanged = false;
        public bool PasswordChanged = false;

        public bool PreviousTwoFA = false;
        public bool TwoFAChanged = false;

        public bool PreviousEmail = false;
        public bool EmailFAChanged = false;
        public bool EmailChanged = false;
        string email;

        public bool PreviousGoogle = false;
        public bool GoogleFAChanged = false;


        public bool out2FA = false;
        public bool outGoogle = false;
        public bool outEmail = false;

        bool error = false;

        PasswordBox pass;

        public Window8(string user, PasswordBox password, string Email, bool TwoFA, bool Emailchecked,bool Googlechecked)
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
            if (TwoFA == true) 
            {
                PreviousTwoFA = true;
                TwoFACheck.IsChecked = true;
                GoogleCheck.Visibility = Visibility.Visible;
                EmailCheck.Visibility = Visibility.Visible;
                if (Emailchecked == true) 
                {
                    PreviousEmail = true;
                    EmailCheck.IsChecked = true;
                    EmailLabel.Visibility = Visibility.Visible;
                    EmailTekst.Visibility = Visibility.Visible;
                    ButtonEmail.Visibility = Visibility.Visible;
                }
                if (Googlechecked == true) 
                {
                    PreviousGoogle = true;
                    GoogleCheck.IsChecked = true;
                }
            }
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

            if (TwoFACheck.IsChecked != PreviousTwoFA) 
            {
                TwoFAChanged = true;
            }

            if (TwoFACheck.IsChecked == false)
            {
                if (PreviousEmail == true)
                {
                    succesfull = true;
                }
                if (PreviousGoogle == true)
                {
                    succesfull = true;
                }
                EmailCheck.IsChecked = false;
                GoogleCheck.IsChecked = false;
            }

            if (EmailCheck.IsChecked != PreviousEmail)
            {
                EmailChanged = true;
            }

            if (GoogleCheck.IsChecked != PreviousGoogle)
            {
                GoogleFAChanged = true;
            }

            if(TwoFACheck.IsChecked == true) 
            {
                if (EmailCheck.IsChecked == true)
                {
                    if (EmailText.Text != email && EmailText.Text != "")
                    {
                        succesfull = true;
                        outEmail = true;
                    }
                    else
                    {
                        error = true;
                        Window2 win2 = new Window2();
                        win2.Title = "Error";
                        win2.Error.Content = "Your email must be different than original.";
                        win2.ShowDialog();
                    }
                }
                if (GoogleCheck.IsChecked == true)
                {
                    succesfull = true;
                    outGoogle = true;
                }
                if (EmailCheck.IsChecked == true && GoogleCheck.IsChecked == true)
                {
                    error = true;
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "You must choose only one form of 2FA.";
                    win2.ShowDialog();
                }
                if (EmailCheck.IsChecked == false && GoogleCheck.IsChecked == false)
                {
                    error = true;
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "You must choose some kind of 2FA.";
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

            if ((PasswordChanged == true || LoginChanged == true || EmailFAChanged == true || GoogleFAChanged == true || EmailChanged == true || TwoFAChanged == true) && error == false)
            {
                this.Close();
            }
            else
            {
                error = false;
                LoginChanged = false;
                EmailChanged = false;
                PasswordChanged = false;

                EmailFAChanged = false;
                GoogleFAChanged = false;
                TwoFAChanged = false;

                out2FA = false;
                outGoogle = false;
                outEmail = false;

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

        private void Checked_2FA(object sender, RoutedEventArgs e)
        {
            GoogleCheck.Visibility = Visibility.Visible;
            EmailCheck.Visibility = Visibility.Visible;
        }
        private void Unchecked_2FA(object sender, RoutedEventArgs e)
        {
            GoogleCheck.Visibility = Visibility.Hidden;
            EmailCheck.Visibility = Visibility.Hidden;
            EmailTekst.Visibility = Visibility.Hidden;
            EmailLabel.Visibility = Visibility.Hidden;
            EmailText.Visibility = Visibility.Hidden;
            ButtonEmail.Visibility = Visibility.Hidden;
        }
        private void Checked_Email(object sender, RoutedEventArgs e)
        {
            EmailTekst.Visibility = Visibility.Visible;
            EmailLabel.Visibility = Visibility.Visible;
            ButtonEmail.Visibility = Visibility.Visible;
        }
        private void Unchecked_Email(object sender, RoutedEventArgs e)
        {
            EmailTekst.Visibility = Visibility.Hidden;
            EmailLabel.Visibility = Visibility.Hidden;
            EmailText.Visibility = Visibility.Hidden;
            ButtonEmail.Visibility = Visibility.Hidden;
        }
    }
}
