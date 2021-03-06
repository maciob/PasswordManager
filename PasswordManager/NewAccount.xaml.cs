﻿using MahApps.Metro.Controls;
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
    public partial class Window6 : MetroWindow
    {
        public bool succesfull = false;
        public bool Flag_TwoFA = false;
        public bool Flag_Email = false;
        public bool Flag_GoogleAuthenticator = false;

        public Window6()
        {
            InitializeComponent();
        }

        private void Button_Create_Account(object sender, RoutedEventArgs e)
        {
            Window5 win5 = new Window5(0, 0, 0, 0, 0, false);
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
            if (win5.Check(Password_Box.Password.ToString(), 1, 1, 1, 1) == true && Password_Box.Password.Length > 8)
            {
                if (Login.Text.Length < 5)
                {
                    Window2 win2 = new Window2();
                    win2.Title = "Error";
                    win2.Error.Content = "Your account name must be at least 5 characters long.";
                    win2.ShowDialog();
                }
                else 
                {
                    if (Flag_TwoFA == true) 
                    {
                        if (Flag_Email == true && Flag_GoogleAuthenticator == true)
                        {
                            Window2 win2 = new Window2();
                            win2.Title = "Error";
                            win2.Error.Content = "You must choose only one form of 2FA.";
                            win2.ShowDialog();
                        }
                        else
                        {
                            succesfull = true;
                            win5.Close();
                            this.Close();
                        }
                    }
                    else 
                    {
                        succesfull = true;
                        win5.Close();
                        this.Close();
                    }
                }
                win5.Close();
            }
            else
            {
                Window2 win2 = new Window2();
                win2.Title = "Error";
                win2.Error.Content = "Your password must consist: At least 8 characters and at least\none number, huge letter, small letter and special character.";
                win2.ShowDialog();
            }
            win5.Close();
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

        private void Checked_2FA(object sender, RoutedEventArgs e)
        {
            Flag_TwoFA = true;
            EmailCheck.Visibility = Visibility.Visible;
            GoogleCheck.Visibility = Visibility.Visible;
        }

        private void Checked_Email(object sender, RoutedEventArgs e)
        {
            Flag_Email = true;
            Email.Visibility = Visibility.Visible;
            EmailLabel.Visibility = Visibility.Visible;
        }

        private void Checked_GoogleAuthenticator(object sender, RoutedEventArgs e)
        {
            Flag_GoogleAuthenticator = true;
        }

        private void Unchecked_2FA(object sender, RoutedEventArgs e)
        {
            Flag_TwoFA = false;
            EmailCheck.Visibility = Visibility.Hidden;
            GoogleCheck.Visibility = Visibility.Hidden;
            Email.Visibility = Visibility.Hidden;
            EmailLabel.Visibility = Visibility.Hidden;
        }

        private void Unchecked_Email(object sender, RoutedEventArgs e)
        {
            Flag_Email = false;
            Email.Visibility = Visibility.Hidden;
            EmailLabel.Visibility = Visibility.Hidden;
        }

        private void Unchecked_GoogleAuthenticator(object sender, RoutedEventArgs e)
        {
            Flag_GoogleAuthenticator = false;
        }
    }
}