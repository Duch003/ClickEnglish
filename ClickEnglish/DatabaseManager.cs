using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Npgsql;


namespace ClickEnglish
{
    public class DatabaseManager
    {
        //Static clause make variables shared via all intances of this class
        private static NpgsqlConnection _connect;
        private static string _connString;
        private static bool _connected;
        

        public DatabaseManager(string server, string user, string password, string port, string dbName)
        {
            _connected = false;
            if (server == null || user == null || password == null || port == null || dbName == null)
                return;
            var connString = $"Server={server};Port={port};User id={user};Password={password};Database={dbName}";
            _connString = connString;
        }

        public DatabaseManager() { }

        #region Connection
        public void Connect()
        {
            if (_connString == null)
                throw new Exception("Cannot connect to database.");
            _connect = new NpgsqlConnection(_connString);
            _connect.Open();
            _connected = true;
        }

        public void Disconnect()
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            _connect.Close();
            _connected = false;
        }

        public bool IsConnected()
        {
            return _connected;
        }
        #endregion

        #region Queries
        internal DataSet Query(string myQuery)
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            var dataAdapter = new NpgsqlDataAdapter($"{myQuery}", _connect);
            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            return dataSet;
        }

        internal void NonQuery(string myQuery)
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            try
            {
                var command = new NpgsqlCommand($"{myQuery}", _connect);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception($"Method: NonQuery.\n\n{e.Message}");
            }
        }

        #endregion

        #region Validation and filtering
        internal string HashString(string arg)
        {
            if (string.IsNullOrEmpty(arg)) throw new Exception("Method: HashString. Input argument were empty.");
            var bytes = Encoding.UTF8.GetBytes(arg);
            var encode = new SHA256Managed();
            var hash = encode.ComputeHash(bytes);

            return hash.Aggregate("", (current, z) => current + $"{z}");
        }

        internal bool Validate(string nick)
        {
            //Checking length of nickname and password
            if (string.IsNullOrEmpty(nick))
            {
                return true;
            }
            //Checking if nickname or password contains forbidden characters
            if (Filter(nick))
            {
                return true;
            }
            return false;
        }

        //Anwser the question: Does input string contains any of forbidden chars?
        internal bool Filter(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return true;
            var forbidden = new[]
            {
                '\'',
                '-',
                ':',
                '\\',
                '/',
                '"',
                '!',
                '@',
                '#',
                '$',
                '%',
                '^',
                '&',
                '*',
                '(',
                ')',
                '+',
                '{',
                '}',
                '_',
                '=',
                '.',
                '|'
            };
            foreach (char z in raw)
            {
                if (forbidden.Contains(z)) return true;
            }
            return false;
        }
        #endregion

        #region TestingFeatures
        public bool RestoreDatabase_USERS()
        {
            Connect();
            try
            {
                NonQuery("TRUNCATE users RESTART IDENTITY");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'Duch003', '3814415224783672242261051221007419411217620821316624920551248522149232117762710170114', 5, true, 600)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'User', '220301243225985712353911112813793253243121131521931114612852331811418423820221842', 30, false, 360)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'Kasm82', '4641066320266217240200129193112249166432063014212710823620212888914816185672866', 60, false, 360)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'User2', '1032251881601361551622823314920346524016024115417305857221205138511101702191561382779', 70, false, 240)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'Anonymus', '451722612622233190151115253432501941581201968914882251225105281622021952132252055104119', 5, true, 30)");
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Method: RestoreDatabase_USERS.\n\n{e.Message}");
            }
        }
        #endregion


        //True if exists
        //Flase if not
        //Exception if disconnected, forbidden chars
        public bool IsUserAlreadyExists(string nick)
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            if (Validate(nick))
                throw new Exception("Method: IsUserAlreadyExists. Some arguments are null or empty or contains forbidden signs.");
            var queryResult = Query($"SELECT user_name, user_password FROM users WHERE user_name = '{nick}'");

            if (queryResult.Tables.Count < 1) return false;
            if (queryResult.Tables[0].Rows.Count == 0) return false;
            else if (queryResult.Tables[0].Rows.Count == 1)
            {
                var tempUser = queryResult.Tables[0].Rows[0][0].ToString(); //User

                if (tempUser.Equals(nick)) return true;
            }
            return false;
        }

        //True if successfull
        //False if user does'nt exists
        //Exception if disconnected, forbidden chars
        public bool TryToLogIn(string nick, string password, out DataSet userData)
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            if (Validate(nick) || Validate(password))
                throw new Exception("Method: TryToLogIn. Some arguments are null or empty or contains forbidden signs.");

            var hashedPassword = HashString(password);
            var queryResult = Query($"SELECT * FROM users WHERE user_name = '{nick}' AND user_password = '{hashedPassword}'");

            if (queryResult.Tables.Count < 1)
            {
                userData = null;
                return false;
            }
            if (queryResult.Tables[0].Rows.Count == 0)
            {
                userData = null;
                return false;
            }
            else if (queryResult.Tables[0].Rows.Count == 1)
            {
                var tempUser = queryResult.Tables[0].Rows[0][1].ToString();     //User
                var tempPassword = queryResult.Tables[0].Rows[0][2].ToString(); //Password

                if (tempUser.Equals(nick) && tempPassword.Equals(hashedPassword))
                {
                    userData = queryResult;
                    return true;
                }
            }
            userData = null;
            return false;
        }

        //True if successeded
        //Flase if users exists
        //Exception when forbidden disconnected, forbidden chars, error while query execution
        public bool RegisterNewUser(string nick, string password)
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            if (Validate(nick) || Validate(password))
                throw new Exception("Method: Register new user. Some arguments are null or empty or contains forbidden signs.");
            if (IsUserAlreadyExists(nick))
                return false;

            var hashedPassword = HashString(password);
            try
            {
                var query = $"INSERT INTO users VALUES (default, '{nick}', '{hashedPassword}', 0, false, 15)";
                NonQuery(query);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Method: RegisterNewUser. An error occured during register process.\n\n{e.Message}");
            }
        }

        //True if succedeed
        //False if user doesnt exists
        //Exception if disconnected, forbidden chars
        public bool DeleteUser(string nick, string password)
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            if (Validate(nick) || Validate(password))
                throw new Exception("Method: Register new user. Some arguments are null or empty or contains forbidden signs.");
            if (IsUserAlreadyExists(nick))
            {
                var hashedPassword = HashString(password);
                NonQuery($"DELETE FROM users WHERE user_name = '{nick}' AND user_password = '{hashedPassword}'");
                return true;
            }
            return false;
        }

        //True if successeded
        public bool SaveSettings()
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            try
            {
                var query = $"UPDATE users SET " +
                    $"vocabularySize = {GlobalSettings.RandomVocabulaySize}, " +
                    $"soundStatus = {GlobalSettings.SoundState}, " +
                    $"timeChallange = {GlobalSettings.Time} " +
                    $"WHERE id = {GlobalSettings.ID}";
                NonQuery(query);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Method: SaveSettings. Cannot save user settings.\n\n{e.Message}");
            }
        }

        //Returns number of words
        public uint CountDictionary(byte user_id)
        {
            if (!_connected)
                throw new Exception("Connection with server is closed.");
            var query = $"SELECT count(id) FROM dictionary WHERE user_id = {user_id}";
            var queryResult = Query(query);


            if (queryResult.Tables.Count == 0)
            {
                throw new Exception($"Method: CountDictionary. There is no tables in return.");
            }
            if (queryResult.Tables[0].Rows.Count == 0)
            {
                return 0;
            }
            else
            {
                if (UInt32.TryParse(queryResult.Tables[0].Rows[0][0].ToString(), out uint counter))
                    return counter;
                return 0;
            }
        }
    }
}
