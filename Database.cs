using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

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
                        "Database not initialized – call Database.Initialize() first");
                return _instance;
            }
        }

        public static Database Initialize()
        {
            if (_instance != null)
                throw new InvalidOperationException("Database already initialized");
            _instance = new Database();
            return _instance;
        }

        private string user;
        private int userId;
        private int userGroupId;
        private string connectionString =
            $"server={ConfigurationManager.AppSettings["server"]};uid={ConfigurationManager.AppSettings["uid"]};pwd={ConfigurationManager.AppSettings["pwd"]};database={ConfigurationManager.AppSettings["database"]}";
        private MySqlConnection connection;

        public bool Connect()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Couldn't connect to db: {e}");
                return false;
            }
        }

        public void Register(string _login, string _password, string _group)
        {
            string login = MySqlHelper.EscapeString(_login);
            string password = MySqlHelper.EscapeString(_password);
            string hash = EncryptString(password);

            string sql = @"INSERT INTO `users` (`login`, `password`) VALUES (@login, @hash);";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.ExecuteNonQuery();
                userId = (int)cmd.LastInsertedId;
            }
            string linkSql = @"INSERT INTO user_groups (user_id, group_id) VALUES (@uid, @gid);";
            using (var cmdLink = new MySqlCommand(linkSql, connection))
            {
                cmdLink.Parameters.AddWithValue("@uid", userId);
                cmdLink.Parameters.AddWithValue("@gid", _group);
                cmdLink.ExecuteNonQuery();
            }
        }
        // MD5 hashing
        public static string EncryptString(string input)
        {
            StringBuilder hash = new StringBuilder();
            using (var md5provider = new MD5CryptoServiceProvider())
            {
                byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));
                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Append(bytes[i].ToString("x2"));
                }
            }
            return hash.ToString();
        }

        public bool Login(string _login, string _password)
        {
            bool loginSuccess = false;
            string login = MySqlHelper.EscapeString(_login);
            string password = MySqlHelper.EscapeString(_password);
            string hash = EncryptString(password);

            string sql = @"SELECT id FROM users WHERE login = @login AND password = @hash";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@login", login);
                cmd.Parameters.AddWithValue("@hash", hash);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        loginSuccess = true;
                        userId = rdr.GetInt32("id");
                    }
                }
            }

            if (!loginSuccess)
                return false;

            user = login;

            string grpSql = @"SELECT group_id FROM user_groups WHERE user_id = @uid LIMIT 1";
            using (var cmd = new MySqlCommand(grpSql, connection))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        userGroupId = rdr.GetInt32("group_id");
                    else
                        userGroupId = 0;
                }
            }

            return true;
        }

        public void InsertEvent(Day day)
        {
            string date = $"{day.day}.{day.month}.{day.year}";
            string nameEsc = MySqlHelper.EscapeString(day.name);
            string descEsc = MySqlHelper.EscapeString(day.description);

            int eventId;
            if (EventExistsForUser(date))
            {
                // UPDATE existing event
                string updateSql = @"
UPDATE events
SET `name` = @name, `description` = @desc
WHERE `date` = @date AND user_id = @uid";
                using (var cmd = new MySqlCommand(updateSql, connection))
                {
                    cmd.Parameters.AddWithValue("@name", nameEsc);
                    cmd.Parameters.AddWithValue("@desc", descEsc);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                }

                eventId = GetEventId(date);
            }
            else
            {
                // INSERT new event
                string insertSql = @"
INSERT INTO events (`user_id`, `date`, `name`, `description`)
VALUES (@uid, @date, @name, @desc);";
                using (var cmd = new MySqlCommand(insertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@name", nameEsc);
                    cmd.Parameters.AddWithValue("@desc", descEsc);
                    cmd.ExecuteNonQuery();
                    eventId = (int)cmd.LastInsertedId;
                }
            }
            if (userGroupId > 0)
            {
                string linkCheck = @"SELECT COUNT(*) FROM event_groups WHERE event_id = @eid AND group_id = @gid";
                using (var cmd = new MySqlCommand(linkCheck, connection))
                {
                    cmd.Parameters.AddWithValue("@eid", eventId);
                    cmd.Parameters.AddWithValue("@gid", userGroupId);
                    var exists = Convert.ToInt32(cmd.ExecuteScalar());
                    if (exists == 0)
                    {
                        string linkSql = @"INSERT INTO event_groups (event_id, group_id) VALUES (@eid, @gid)";
                        using (var linkCmd = new MySqlCommand(linkSql, connection))
                        {
                            linkCmd.Parameters.AddWithValue("@eid", eventId);
                            linkCmd.Parameters.AddWithValue("@gid", userGroupId);
                            linkCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private bool EventExistsForUser(string date)
        {
            string sql = @"SELECT COUNT(*) FROM events WHERE `date` = @date AND user_id = @uid";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@uid", userId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private int GetEventId(string date)
        {
            string sql = @"SELECT id FROM events WHERE `date` = @date AND user_id = @uid LIMIT 1";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@uid", userId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool UserExists(string _login)
        {
            string login = MySqlHelper.EscapeString(_login);
            string sql = @"SELECT COUNT(*) FROM users WHERE login = @login";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@login", login);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public string[] LoadDays(int month, int year)
        {
            string sql = @"
                SELECT DISTINCT
                    e.`date`,
                    e.`name`,
                    e.`description`
                FROM events e
                    LEFT JOIN event_groups eg ON eg.event_id = e.id
                    LEFT JOIN user_groups ug  ON ug.group_id = eg.group_id
                WHERE
                    MONTH(STR_TO_DATE(e.`date`, '%d.%m.%Y')) = @month
                    AND YEAR(STR_TO_DATE(e.`date`, '%d.%m.%Y')) = @year
                    AND (e.user_id = @id OR ug.user_id = @id)";

            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@id", userId);

                var list = new List<string>();
                using (var rdr = cmd.ExecuteReader())
                {
                    int idxDate = rdr.GetOrdinal("date");
                    int idxName = rdr.GetOrdinal("name");
                    int idxDesc = rdr.GetOrdinal("description");

                    while (rdr.Read())
                    {
                        string dateStr = rdr.IsDBNull(idxDate) ? string.Empty : rdr.GetString(idxDate);
                        string day = string.Empty;
                        if (!string.IsNullOrEmpty(dateStr))
                        {
                            var parts = dateStr.Split('.');
                            day = parts.Length == 3 ? parts[0] : dateStr;
                        }
                        string name = rdr.IsDBNull(idxName) ? string.Empty : rdr.GetString(idxName);
                        string description = rdr.IsDBNull(idxDesc) ? string.Empty : rdr.GetString(idxDesc);

                        list.Add($"{day},{name},{description}");
                    }
                }
                return list.ToArray();
            }
        }

        public void DeleteDay(string date)
        {
            string sql = @"DELETE FROM events WHERE `date` = @date AND user_id = @uid";
            using (var cmd = new MySqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public void Close()
        {
            connection?.Close();
        }
    }
}
