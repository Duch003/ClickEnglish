using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClickEnglish;
using System.Threading.Tasks;
using System.Data;

namespace ClickEnglish_IntegrationTests
{
    [TestFixture]
    class DatabaseManagaer_IntegrationTests
    {
        DatabaseManager testManager = new DatabaseManager("localhost", "Duch003", "Killer003", "5432", "MyDictionaryApp_IntegrationTests");

        #region IsUserAlreadyExists
        [TestCase("Duch003", true)]
        [TestCase("User", true)]
        [TestCase("Anonymus", true)]
        [TestCase("Not", false)]
        public void IsUserAlreadyExists_InnerLogicTest(string user, bool expected)
        {
            testManager.Connect();
            testManager.RestoreDatabase_USERS();
            var result = testManager.IsUserAlreadyExists(user);
            Assert.IsTrue(result == expected);
            testManager.Disconnect();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("!%$")]
        public void IsUserAlreadyExists_ThrowExceptionWhenInputInvalid(string user)
        {
            testManager.Connect();
            Assert.Throws<Exception>(() => testManager.IsUserAlreadyExists(user));
            testManager.Disconnect();
        }
        #endregion

        #region TryToLogIn
        [TestCase("Duch003", "Killer003", true)]
        [TestCase("User", "MyPassword", true)]
        [TestCase("Kasm82", "StillNotEmpty", true)]
        [TestCase("Kasm82", "Lolno", false)]
        [TestCase("User2", "Nothing", true)]
        [TestCase("Kasm82", "Nothing", false)]
        [TestCase("Anonymus", "Lolno", true)]
        [TestCase("BadUser", "BadPassword", false)]
        [TestCase("BadUser", "Killer003", false)]
        [TestCase("Kasm82", "BadPassword", false)]
        public void TryToLogIn_InnerLogicTest(string nick, string password, bool expected)
        {
            testManager.Connect();
            var result = testManager.TryToLogIn(nick, password, out DataSet temp);
            Assert.True(expected == result);
            testManager.Disconnect();
        }

        [TestCase("", "Killer003")]
        [TestCase("Anonymus", "Lol$no")]
        [TestCase("User", "")]
        [TestCase("%!#", "Lolno")]
        [TestCase(null, "Killer003")]
        [TestCase(null, "")]
        [TestCase(null, null)]
        [TestCase("", "")]
        public void TryToLogIn_ThrowExceptionWhenInvalidInput(string nick, string password)
        {
            testManager.Connect();
            Assert.Throws<Exception>(() => testManager.TryToLogIn(nick, password, out DataSet temp));
            testManager.Disconnect();
        }
        #endregion

        #region DeleteUser
        [TestCase("Duch003", "Killer003", true)]
        [TestCase("BadUser", "Killer003", false)]
        [TestCase("User", "MyPassword", true)]
        [TestCase("Duch003", "BadPassword", false)]
        [TestCase("Kasm82", "StillNotEmpty", true)]
        [TestCase("User2", "Nothing", true)]
        [TestCase("Anonymus", "Lolno", true)]
        public void DeleteUser_InnerLogicTest(string nick, string password, bool expected)
        {
            testManager.Connect();
            var result = testManager.DeleteUser(nick, password);
            var existanceTest = testManager.IsUserAlreadyExists(nick);
            Assert.IsTrue(result == expected && !existanceTest);
        }

        [TestCase("Duch003", "Killer003", false)]
        [TestCase("BadUser", "Killer003", false)]
        [TestCase("Duch003", "BadPassword", false)]
        [TestCase("BadLogin", "BadPassword", false)]
        public void DeleteUser_UsersDoesntExist_ReturnFalse(string nick, string password, bool expected)
        {
            testManager.Connect();
            var result = testManager.DeleteUser(nick, password);
            var existanceTest = testManager.IsUserAlreadyExists(nick);
            Assert.IsTrue(result == expected && !existanceTest);
        }

        [TestCase("!@#%$", "BadPassword")]
        [TestCase("!@#%$", "")]
        [TestCase("", "BadPassword")]
        [TestCase("BadLogin", "(%&")]
        [TestCase(null, "BadPassword")]
        [TestCase("Duch003", null)]
        [TestCase(null, null)]
        public void DeleteUser_ThrowExceptionWhenInvalidInput(string nick, string password)
        {
            testManager.Connect();
            Assert.Throws<Exception>(() => testManager.DeleteUser(nick, password));
            testManager.Disconnect();
        }
        #endregion

        #region RegisterNewUser
        [TestCase("MyNewUser1", "MyPassword2", true)]
        [TestCase("MyNewUser2", "MyPassword4", true)]
        [TestCase("MyNewUser3", "MyPassword6", true)]
        public void RegisterNewUser_InnerLogicTest(string nick, string password, bool expected)
        {
            testManager.Connect();
            bool result = testManager.RegisterNewUser(nick, password);
            bool existanceTest = testManager.IsUserAlreadyExists(nick);
            testManager.DeleteUser(nick, password);
            Assert.IsTrue(expected == result && existanceTest);
        }
        #endregion

        #region CountDictionary
        [TestCase(1, 3)]
        [TestCase(2, 5)]
        [TestCase(3, 5)]
        [TestCase(4, 4)]
        [TestCase(5, 6)]
        [TestCase(50, 0)]
        public void CountDictionary_ReturnWordsById(byte id, byte expected)
        {
            testManager.Connect();
            var result = testManager.CountDictionary(id);
            Assert.IsTrue(result == expected);
        }
        #endregion

        #region DictionaryManager
        [TestCase(1, 3, true)]
        [TestCase(2, 5, true)]
        [TestCase(3, 5, true)]
        [TestCase(4, 4, true)]
        [TestCase(5, 6, true)]
        [TestCase(20, 0, false)]
        public void TakeDictionary_InnerLogicTest(int id, int expectedCount, bool expectedResult)
        {
            testManager.Connect();
            bool result = testManager.TakeDictionary(id, out DataSet temp);
            if (temp == null)
                Assert.IsTrue(result == expectedResult);
            else
                Assert.IsTrue(temp.Tables[0].Rows.Count == expectedCount && result == expectedResult);
        }

        [TestCase()]
        public void TakeDictionary_WordCondition_InnerLogicTest(int id, string condition, int expectedCount, bool expectedResult) {
            testManager.Connect();
            bool result = testManager.TakeDictionary_WordCondition(id, out DataSet temp, condition);
            if(temp == null)
                Assert.IsTrue(result == expectedResult);
            else
                Assert.IsTrue(temp.Tables[0].Rows.Count == expectedCount && result == expectedResult);
        }

        public void TakeDictionary_WordCondition_ThrowException(int id, string condition, int expectedCount, bool expectedResult) {
            testManager.Connect();
            Assert.Throws<Exception>(() => testManager.TakeDictionary_WordCondition(id, out DataSet temp, condition));
        }

        public void TakeCategories_InnerLogicTest(int id, int expectedCount, bool expectedResult) {

        }

        public void AddNewRecord_InnerLogicTest(int id, int expectedCount, bool expectedResult) {

        }

        public void UpdateRecord_InnerLogicTest(int id, int expectedCount, bool expectedResult) {

        }

        public void RemoveRecord_InnerLogicTest(int id, int expectedCount, bool expectedResult) {

        }
        #endregion
    }
}
