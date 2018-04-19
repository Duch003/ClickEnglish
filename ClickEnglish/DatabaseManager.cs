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


namespace ClickEnglish {
    //TODO Czy uczynik ta klase statyczna? I polaczaczyc z GlobalSettings?
    public class DatabaseManager {
        //Static clause make variables shared via all intances of this class
        private static NpgsqlConnection _connect;
        private static string _connString;
        private static bool _connected;

        public DatabaseManager(string server, string user, string password, string port, string dbName) {
            _connected = false;
            if(server == null || user == null || password == null || port == null || dbName == null)
                return;
            var connString = $"Server={server};Port={port};User id={user};Password={password};Database={dbName}";
            _connString = connString;
        }

        public DatabaseManager() { }

        #region Connection
        public void Connect() {
            if(_connString == null)
                throw new Exception("Cannot connect to database.");
            _connect = new NpgsqlConnection(_connString);
            _connect.Open();
            _connected = true;
        }

        public void Disconnect() {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            _connect.Close();
            _connected = false;
        }

        public bool IsConnected() {
            return _connected;
        }
        #endregion

        #region Queries
        internal DataSet Query(string myQuery) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            var dataAdapter = new NpgsqlDataAdapter($"{myQuery}", _connect);
            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
            return dataSet;
        }

        internal void NonQuery(string myQuery) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            try {
                var command = new NpgsqlCommand($"{myQuery}", _connect);
                command.ExecuteNonQuery();
            } catch(Exception e) {
                throw new Exception($"Method: NonQuery.\n\n{e.Message}");
            }
        }

        #endregion

        #region Validation and filtering
        internal string HashString(string arg) {
            if(string.IsNullOrEmpty(arg))
                throw new Exception("Method: HashString. Input argument were empty.");
            var bytes = Encoding.UTF8.GetBytes(arg);
            var encode = new SHA256Managed();
            var hash = encode.ComputeHash(bytes);

            return hash.Aggregate("", (current, z) => current + $"{z}");
        }

        //Return true of contains forbidden chars
        //Flase if string clear
        internal bool Validate(string nick) {
            //Checking length of nickname and password
            if(string.IsNullOrEmpty(nick)) {
                return true;
            }
            //Checking if nickname or password contains forbidden characters
            if(Filter(nick)) {
                return true;
            }
            return false;
        }

        //Anwser the question: Does input string contains any of forbidden chars?
        internal bool Filter(string raw) {
            if(string.IsNullOrEmpty(raw))
                return true;
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
            foreach(char z in raw) {
                if(forbidden.Contains(z))
                    return true;
            }
            return false;
        }
        #endregion

        #region TestingFeatures
        public bool RestoreDatabase_USERS() {
            Connect();
            try {
                NonQuery("TRUNCATE users RESTART IDENTITY");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'Duch003', '3814415224783672242261051221007419411217620821316624920551248522149232117762710170114', 5, true, 600)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'User', '220301243225985712353911112813793253243121131521931114612852331811418423820221842', 30, false, 360)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'Kasm82', '4641066320266217240200129193112249166432063014212710823620212888914816185672866', 60, false, 360)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'User2', '1032251881601361551622823314920346524016024115417305857221205138511101702191561382779', 70, false, 240)");
                NonQuery("INSERT INTO users VALUES(DEFAULT, 'Anonymus', '451722612622233190151115253432501941581201968914882251225105281622021952132252055104119', 5, true, 30)");
                return true;
            } catch(Exception e) {
                throw new Exception($"Method: RestoreDatabase_USERS.\n\n{e.Message}");
            }
        }

        public bool RestoreDatabase_DICTIONARY()
        {
            Connect();
            try {
                NonQuery("TRUNCATE dictionary RESTART IDENTITY");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'testeng', 'test', 1, 'image1', 6, 3);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test1eng', 'test1', 0.2, 'image1', 5, 5);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test2eng', 'test2', 0.32,'image2', 2, 2);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test3eng', 'test3', 0.7,'image1', 3, 3);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test4eng', 'test4', 1,'image3', 7, 4);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test5eng', 'test5', 0.67,'image3', 2, 2);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test6eng', 'test6', 0.98,'image3', 1, 1);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test7eng', 'test7', 0.01,'image4', 5, 5);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test8eng', 'test8', 0.11,'image2', 5, 5);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test9eng', 'test9', 0.45,'image1', 5, 5);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test10eng', 'test10', 1,'image4', 4, 4);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test11eng', 'test11', 0,'image3', 2, 2);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test12eng', 'test12', 0.14,'image4', 3, 3);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test13eng', 'test13', 0.56,'image4', 5, 5);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test14eng', 'test14', 0.91,'image2', 1, 1);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test15eng', 'test15', 1,'image2', 1, 1);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test16eng', 'test16', 0.62,'image1', 2, 2);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test17eng', 'test17', 0.88,'image3', 3, 3);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test18eng', 'test18', 0.19,'image1', 2, 2);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test19eng', 'test19', 0.73,'image4', 4, 4);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test20eng', 'test20', 0.44,'image5', 3, 3);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test21eng', 'test21', 0.88,'image2', 5, 5);");
                NonQuery("INSERT INTO dictionary VALUES(DEFAULT, 'test22eng', 'test22', 0,'image3', 4, 4);");
                return true;
            } catch(Exception e) {
                throw new Exception($"Method: RestoreDatabase_USERS.\n\n{e.Message}");
            }    
        }
        #endregion

        #region Settings and accounts
        //True if exists
        //Flase if not
        //Exception if disconnected, forbidden chars
        public bool IsUserAlreadyExists(string nick) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(nick))
                throw new Exception("Method: IsUserAlreadyExists. Some arguments are null or empty or contains forbidden signs.");
            var queryResult = Query($"SELECT user_name, user_password FROM users WHERE user_name = '{nick}'");

            if(queryResult.Tables.Count < 1)
                return false;
            if(queryResult.Tables[0].Rows.Count == 0)
                return false;
            else if(queryResult.Tables[0].Rows.Count == 1) {
                var tempUser = queryResult.Tables[0].Rows[0][0].ToString(); //User

                if(tempUser.Equals(nick))
                    return true;
            }
            return false;
        }

        //True if successfull
        //False if user does'nt exists
        //Exception if disconnected, forbidden chars
        public bool TryToLogIn(string nick, string password, out DataSet userData) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(nick) || Validate(password))
                throw new Exception("Method: TryToLogIn. Some arguments are null or empty or contains forbidden signs.");

            var hashedPassword = HashString(password);
            var queryResult = Query($"SELECT * FROM users WHERE user_name = '{nick}' AND user_password = '{hashedPassword}'");

            if(queryResult.Tables.Count < 1) {
                userData = null;
                return false;
            }
            if(queryResult.Tables[0].Rows.Count == 0) {
                userData = null;
                return false;
            } else if(queryResult.Tables[0].Rows.Count == 1) {
                var tempUser = queryResult.Tables[0].Rows[0][1].ToString();     //User
                var tempPassword = queryResult.Tables[0].Rows[0][2].ToString(); //Password

                if(tempUser.Equals(nick) && tempPassword.Equals(hashedPassword)) {
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
        public bool RegisterNewUser(string nick, string password) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(nick) || Validate(password))
                throw new Exception("Method: Register new user. Some arguments are null or empty or contains forbidden signs.");
            if(IsUserAlreadyExists(nick))
                return false;

            var hashedPassword = HashString(password);
            try {
                var query = $"INSERT INTO users VALUES (default, '{nick}', '{hashedPassword}', 0, false, 15)";
                NonQuery(query);
                return true;
            } catch(Exception e) {
                throw new Exception($"Method: RegisterNewUser. An error occured during register process.\n\n{e.Message}");
            }
        }

        //True if succedeed
        //False if user doesnt exists
        //Exception if disconnected, forbidden chars
        public bool DeleteUser(string nick, string password) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(nick) || Validate(password))
                throw new Exception("Method: Register new user. Some arguments are null or empty or contains forbidden signs.");
            if(IsUserAlreadyExists(nick)) {
                var hashedPassword = HashString(password);
                NonQuery($"DELETE FROM users WHERE user_name = '{nick}' AND user_password = '{hashedPassword}'");
                return true;
            }
            return false;
        }

        //True if successeded
        public bool SaveSettings() {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            try {
                var query = $"UPDATE users SET " +
                    $"vocabularySize = {GlobalSettings.RandomVocabulaySize}, " +
                    $"soundStatus = {GlobalSettings.SoundState}, " +
                    $"timeChallange = {GlobalSettings.Time} " +
                    $"WHERE id = {GlobalSettings.ID}";
                NonQuery(query);
                return true;
            } catch(Exception e) {
                throw new Exception($"Method: SaveSettings. Cannot save user settings.\n\n{e.Message}");
            }
        }
        #endregion

        //Returns number of words
        public int CountDictionary(int user_id) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            var query = $"SELECT count(id) FROM dictionary WHERE user_id = {user_id}";
            var queryResult = Query(query);


            if(queryResult.Tables.Count == 0) {
                throw new Exception($"Method: CountDictionary. There is no tables in return.");
            }
            if(queryResult.Tables[0].Rows.Count == 0) {
                return 0;
            } else {
                if(Int32.TryParse(queryResult.Tables[0].Rows[0][0].ToString(), out int counter))
                    return counter;
                return 0;
            }
        }

        #region DictionaryManager
        //Download whole dictionary for explicit user
        //True if downloaded correctly
        //False if empty
        public bool TakeDictionary(int actualUserId, out DataSet dictionaryData) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            var query = $"SELECT dictionary.id, " + //[0][0] WORD ID
                $"dictionary.eng, " +               //[0][1] ENG
                $"dictionary.pl, " +                //[0][2] PL
                $"dictionary.percentage, " +        //[0][3] %
                $"dictionary.image, " +             //[0][4] IMG
                $"categories.category_name, " +     //[0][5] CATEGORY
                $"categories.id " +                 //[0][6] CATEGORY ID
                $"FROM dictionary " +
                $"INNER JOIN categories ON dictionary.category_id = categories.id " +
                $"WHERE dictionary.user_id = {actualUserId}";
            var queryResult = Query(query);
            if(queryResult.Tables.Count == 0) {
                throw new Exception($"Method: TakeDictionary. There is no tables in return.");
            }
            if(queryResult.Tables[0].Rows.Count == 0) {
                dictionaryData = null;
                return false;
            } else {
                dictionaryData = queryResult;
                return true;
            }
        }

        //Filter method
        //True if downloaded correctly
        //Flase if empty
        //Exception if forbidden chars
        public bool TakeDictionary_WordCondition(int actualUserId, out DataSet dictionaryData, string word) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(word))
                throw new Exception("Method: TakeDictionary_WordCondition. Some arguments are null or empty or contains forbidden signs.");
            var query = $"SELECT dictionary.id, " + //[0][0] WORD ID
                $"dictionary.eng, " +               //[0][1] ENG
                $"dictionary.pl, " +                //[0][2] PL
                $"dictionary.percentage, " +        //[0][3] %
                $"dictionary.image, " +             //[0][4] IMG
                $"categories.category_name, " +     //[0][5] CATEGORY
                $"categories.id " +                 //[0][6] CATEGORY ID
                $"FROM dictionary " +
                $"INNER JOIN categories ON dictionary.category_id = categories.id " +
                $"WHERE dictionary.user_id = {actualUserId}" +
                $" AND (dictionary.pl LIKE '%{word}%' OR dictionary.eng LIKE '%{word}%');";
            var queryResult = Query(query);
            if(queryResult.Tables.Count == 0) {
                throw new Exception($"Method: TakeDictionary_WoedCondition. There is no tables in return.");
            }
            if(queryResult.Tables[0].Rows.Count == 0) {
                dictionaryData = null;
                return false;
            } else {
                dictionaryData = queryResult;
                return true;
            }
        }

        

        #region Manage dictionary records
        //True if successfull
        //Exception if forbidden signs
        public bool AddNewRecord(int actualUserId, Question newWord)
        {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(newWord.ImgSrc) || Validate(newWord.WordPl) || Validate(newWord.WordEng) || Validate(newWord.Cat.Name))
                throw new Exception("Method: AddNewRecord. Some arguments are null or empty or contains forbidden signs.");
            var query = $"INSERT INTO dictionary VALUES (DEFAULT, '{newWord.WordEng}', '{newWord.WordPl}', {newWord.Percentage}, '{newWord.ImgSrc}', {newWord.Cat.Id}, {actualUserId})";
            NonQuery(query);
            return true;
        }

        public bool UpdateRecord(Question updatedWord)
        {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(updatedWord.ImgSrc) || Validate(updatedWord.WordPl) || Validate(updatedWord.WordEng))
                throw new Exception("Method: UpdateRecord. Some arguments are null or empty or contains forbidden signs.");
            var query = $"UPDATE dictionary SET " +
                $"eng = '{updatedWord.WordEng}', " +
                $"pl = '{updatedWord.WordPl}', " +
                $"percentage = {updatedWord.Percentage}, " +
                $"image = '{updatedWord.ImgSrc}', " +
                $"category_id = {updatedWord.Cat.Id}" +
                $"WHERE id = {updatedWord.ID}";
                
            NonQuery(query);
            return true;
        }

        public bool RemoveRecord(int ID)
        {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            var query = $"DELETE FROM dictionary WHERE id = {ID}";
            NonQuery(query);
            return true;
        }
        #endregion


        #endregion

        #region CategoryManager
        //Download whole categories table for explicit user
        //True if downloaded correctly
        //False if empty
        public bool TakeCategories(int actualUserId, out DataSet categoriesData) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            var query = $"SELECT id, category_name FROM categories WHERE user_id = {actualUserId}";
            var queryResult = Query(query);
            if(queryResult.Tables.Count == 0) {
                throw new Exception($"Method: TakeCategories. There is no tables in return.");
            }
            if(queryResult.Tables[0].Rows.Count == 0) {
                categoriesData = queryResult;
                return false;
            } else {
                categoriesData = queryResult;
                return true;
            }
        }
        #endregion

        #region Manager categories records
        //True if successfull
        //Exception if forbidden signs
        public bool AddNewCategory(int actualUserId, Category newCategory) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(newCategory.Name))
                throw new Exception("Method: AddNewCategory. Some arguments are null or empty or contains forbidden signs.");
            var query = $"INSERT INTO categories VALUES (DEFAULT, '{newCategory.Name}', {actualUserId})";
            NonQuery(query);
            return true;
        }

        public bool UpdateCategory(Category updatedCategory) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            if(Validate(updatedCategory.Name))
                throw new Exception("Method: UpdateCategory. Some arguments are null or empty or contains forbidden signs.");
            var query = $"UPDATE dictionary SET " +
                $"category_name = '{updatedCategory.Name}'" +
                $"WHERE id = {updatedCategory.Id}";
            NonQuery(query);
            return true;
        }

        public bool RemoveCategory(int ID) {
            if(!_connected)
                throw new Exception("Connection with server is closed.");
            var query = $"DELETE FROM category WHERE id = {ID}";
            NonQuery(query);
            return true;
        }
        #endregion

    }
}
