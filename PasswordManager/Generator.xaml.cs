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
using System.Security.Cryptography;
using System.IO;
using MahApps.Metro.Controls;

namespace PasswordManager
{
    public partial class Window5 : MetroWindow
    {
        int Length;
        int Lowercase;
        int Uppercase;
        int Special_char;
        int Numbers;
        public Window5(int length, int lowercase, int uppercase, int special_char, int numbers,bool Generator)
        {
            InitializeComponent();
            Length = length;
            Lowercase = lowercase;
            Uppercase = uppercase;
            Special_char = special_char;
            Numbers = numbers;
            if (Generator == true)
            {
                password_box.Password = Generate();
            }
            succesfull = false;
        }

        private RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public bool succesfull = false;

        public string Generate()
        {
            int sum = Lowercase + Uppercase + Special_char + Numbers;
            string results = "";
            do
            {
                results = "";
                for (int x = 0; x < Length; x++)
                {
                    byte num = RandomNum((byte)sum);
                    if (num == 1 && Lowercase == 1)
                    {
                        results = results + GetUniqueLower();
                    }
                    else if (Uppercase == 1 && ((Lowercase == 1 && num == 2) || (Lowercase == 0 && num == 1)))
                    {
                        results = results + GetUniqueUpper();
                    }
                    else if (Numbers == 1 && ((num == 3 && Lowercase == 1 && Uppercase == 1) || (num == 2 && ((Lowercase == 1 && Uppercase == 0) || (Lowercase == 0 && Uppercase == 1))) || num == 1))
                    {
                        results = results + GetUniqueNum();
                    }
                    else
                    {
                        results = results + GetUniqueSpecial();
                    }
                }
            }
            while (!Check(results, Lowercase, Uppercase, Numbers, Special_char));
            rngCsp.Dispose();
            return results;
        }

        private byte RandomNum(byte maxNum)
        {
            if (maxNum <= 0)
                throw new ArgumentOutOfRangeException("maxNum");
            byte[] RandomNumber = new byte[1];
            rngCsp.GetBytes(RandomNumber);
            return (byte)((RandomNumber[0] % maxNum) + 1);
        }

        private string GetUniqueLower()
        {
            char[] chars = new char[26];
            chars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(data);
            StringBuilder result = new StringBuilder(1);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private string GetUniqueUpper()
        {
            char[] chars = new char[26];
            chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(data);
            StringBuilder result = new StringBuilder(1);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private string GetUniqueNum()
        {
            char[] chars = new char[10];
            chars = "1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(data);
            StringBuilder result = new StringBuilder(1);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private string GetUniqueSpecial()
        {
            char[] chars = new char[27];
            chars = "!@#$%^&*()-_=+[]{}:;'<>,.?/".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetBytes(data);
            StringBuilder result = new StringBuilder(1);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public bool Check(string result, int lowercase, int uppercase, int numbers, int special_char)
        {
            bool flag_Check_spec = false;
            bool flag_Check_lower = false;
            bool flag_Check_upper = false;
            bool flag_Check_num = false;
            char[] chars_spec = new char[27];
            chars_spec = "!@#$%^&*()-_=+[]{}:;'<>,.?/".ToCharArray();
            char[] chars_num = new char[10];
            chars_num = "1234567890".ToCharArray();
            char[] chars_upper = new char[26];
            chars_upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] chars_lower = new char[26];
            chars_lower = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            for (int i = 0; i < result.Length; i++)
            {
                if (special_char == 1)
                {
                    for (int j = 0; j < 27; j++)
                    {
                        if (result[i] == chars_spec[j])
                            flag_Check_spec = true;
                    }
                }
                if (uppercase == 1)
                {
                    for (int j = 0; j < 26; j++)
                    {
                        if (result[i] == chars_upper[j])
                            flag_Check_upper = true;
                    }
                }
                if (lowercase == 1)
                {
                    for (int j = 0; j < 26; j++)
                    {
                        if (result[i] == chars_lower[j])
                            flag_Check_lower = true;
                    }
                }
                if (numbers == 1)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (result[i] == chars_num[j])
                            flag_Check_num = true;
                    }
                }
            }
            if (((special_char == 1 && flag_Check_spec == true) || special_char == 0) && ((numbers == 1 && flag_Check_num == true) || numbers == 0) && ((lowercase == 1 && flag_Check_lower == true) || lowercase == 0) && ((uppercase == 1 && flag_Check_upper == true) || uppercase == 0))
                return true;
            else
                return false;
        }

        private void Button_Show(object sender, RoutedEventArgs e)
        {
            if (password_text.Visibility == System.Windows.Visibility.Collapsed)
            {
                password_text.Text = password_box.Password.ToString();
                password_text.Visibility = System.Windows.Visibility.Visible;
                password_box.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                password_box.Password = password_text.Text;
                password_box.Visibility = System.Windows.Visibility.Visible;
                password_text.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            if (password_text.Visibility == System.Windows.Visibility.Collapsed)
            {
                password_text.Text = password_box.Password.ToString();
                password_text.Visibility = System.Windows.Visibility.Visible;
                password_box.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                password_box.Password = password_text.Text;
                password_box.Visibility = System.Windows.Visibility.Visible;
                password_text.Visibility = System.Windows.Visibility.Collapsed;
            }
            succesfull = true;
            this.Close();
        }
    }
}
