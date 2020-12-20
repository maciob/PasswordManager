using System;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Windows.Media.Imaging;

namespace PasswordManager
{
    class DataStructures
    {
        public class Website
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string Website_name { get; set; }
            public string Website_address { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string Date { get; set; }
            public int BackupID { get; set; }
        }
        public class Account
        {
            [Indexed]
            public string Password { get; set; }
            public string Email { get; set; }
            public string Code { get; set; }
        }
        public class Backup
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string Date { get; set; }
        }
    }
}
