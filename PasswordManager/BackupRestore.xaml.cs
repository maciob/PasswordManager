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
using SQLite;
namespace PasswordManager
{
    /// <summary>
    /// Logika interakcji dla klasy Window9.xaml
    /// </summary>
    public partial class Window9 : MetroWindow
    {
        public bool succesfull = false;
        public int choosenBackup;
        public string name;
        public Window9(string login)
        {
            InitializeComponent();
            var db = new SQLiteConnection(login);
            var query = db.Table<DataStructures.Backup>();
            foreach (var backup in query) 
            {
                Combo.Items.Add(backup.Date);
            }
            name = login;
        }
        
        private void Button_OK(object sender, RoutedEventArgs e)
        {
            if (Combo.SelectedItem != null)
            {   
                succesfull = true;
                var db = new SQLiteConnection(name);
                var query = db.Table<DataStructures.Backup>();
                foreach (var backup in query)
                    if(Combo.SelectedItem.ToString()==backup.Date)
                        choosenBackup = backup.ID;
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
