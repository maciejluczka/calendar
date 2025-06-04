using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    internal class Database
    {
        private static Database _instance;

        public static Database Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        "Sidebar not initialized – call Sidebar.Initialize(form) first");
                return _instance;
            }
        }

        public static Database Initialize()
        {
            if (_instance != null)
                throw new InvalidOperationException("Sidebar already initialized");
            _instance = new Database();
            return _instance;
        }

        private string user;
        private string connectionString = "server=127.0.0.1;uid=root;pwd=;database=calendar";
        private MySqlConnection connection;
        public bool Connect()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                string sql = "SELECT * FROM users";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0] + " -- " + rdr[1] + " -- " + rdr[2] + " -- " + rdr[3]);
                }
                rdr.Close();
                return true;
            }catch(Exception e)
            {
                Console.WriteLine($"Couldnt connect to db: {e}");
                return false;
            }
        }


        public void Register(string _login, string _password)
        {
            string login = MySqlHelper.EscapeString(_login);
            string password = MySqlHelper.EscapeString(_password);
            string hash = EncryptString(password);
            string sql = $"INSERT INTO `users` (`login`, `password`, `eventsTable`) VALUES ('{login}', '{hash}', 'events{login}'); ";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();


            //create table
            string name = $"events{login}";
            string query = $@"CREATE TABLE `{name}` (`date` VARCHAR(20),`name` VARCHAR(255),`description` VARCHAR(255));";

            MySqlCommand tableCreate = new MySqlCommand(query, connection);
            tableCreate.ExecuteNonQuery();
        }

        //md5
        public static string EncryptString(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public bool Login(string _login, string _password)
        {
            bool loginSuccess = false;
            string login = MySqlHelper.EscapeString(_login);
            string password = MySqlHelper.EscapeString(_password);
            string hash = EncryptString(password);
            string sql = $"SELECT * FROM users WHERE login = '{login}' AND password = '{hash}'";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                if (rdr[0] != null)
                {
                    loginSuccess = true;
                }
            }
            rdr.Close();
            if (loginSuccess)
                user = login;

            return loginSuccess;
        }

        private bool EventExists(string date)
        {
            string sql = $"SELECT * FROM events{user} WHERE date = '{date}';";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();

            bool exists = false;

            while (rdr.Read())
            {
                if (rdr[0] != null)
                {
                    exists = true;
                }
            }
            rdr.Close();

            return exists;
        }


        public void InsertEvent(Day day)
        {
            string date = $"{day.day}.{day.month}.{day.year}";
            string nameEsc = MySqlHelper.EscapeString(day.name);
            string descEsc = MySqlHelper.EscapeString(day.description);
            string tableName = $"events{user}";

            if (EventExists(date))
            {
                // UPDATE existing event
                string sql = $@"UPDATE `{tableName}`SET `name` = '{nameEsc}',`description` = '{descEsc}'WHERE `date` = '{date}';";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                // INSERT new event
                string sql = $@"INSERT INTO `{tableName}` (`date`, `name`, `description`) VALUES ('{date}', '{nameEsc}', '{descEsc}');";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public bool UserExists(string _login)
        {
            string login = MySqlHelper.EscapeString(_login);
            string sql = $"SELECT * FROM users WHERE login = '{login}'";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader rdr = cmd.ExecuteReader();

            bool exists = false;

            while (rdr.Read())
            {
                if (rdr[0] != null)
                {
                    exists = true;
                }
            }
            rdr.Close();

            return exists;
        }

        //returns events data, day,name,description
        public string[] LoadDays(int month, int year)
        {
            string tableName = $"events{user}";

            string sql = $@"SELECT * FROM `{tableName}`WHERE MONTH(STR_TO_DATE(`date`, '%d.%m.%Y')) = @month AND YEAR (STR_TO_DATE(`date`, '%d.%m.%Y')) = @year;";

            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);

                var list = new List<string>();

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string[] parts = rdr.GetString(0).Split('.');
                        string day = parts.Length == 3 ? parts[0] : rdr.GetString(0);

                        string name = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);
                        string description = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2);

                        list.Add($"{day},{name},{description}");
                    }
                }

                return list.ToArray();
            }
        }

        public void DeleteDay(string date)
        {
            string sql = $"DELETE FROM events{user} WHERE date = '{date}';";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }


        public void Close()
        {
            connection.Close();
        }

    }
}
