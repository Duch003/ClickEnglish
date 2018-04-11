using System;
using System.Collections.Generic;
using System.Data;
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

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private DatabaseManager _manager;
        public LogInWindow()
        {
            InitializeComponent();
            _manager = new DatabaseManager("localhost", "Duch003", "Killer003", "5432", "MyDictionaryApp_IntegrationTests");
            try
            {
                _manager.Connect();
            }
            catch (Exception e)

            {
                MessageBox.Show(e.Message, "Connection error.", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

        //SINGING UP
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            if (!_manager.IsConnected())
            {
                MessageBox.Show("Cannot connect to server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //True if exists
            if (_manager.IsUserAlreadyExists(txtUser.Text))
            {
                MessageBox.Show("User already exists.", "Cannot sign up", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                _manager.RegisterNewUser(txtUser.Text, txtPassword.Password);
            }
            catch (Exception ex)
            {
                throw new Exception($"Method: SignUp_Click. An error occured during registration process.\n\n{ex.Message}");
            }
            MessageBox.Show("Successfully registered. You can now sing in.", "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //SINGING IN
        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            if (!_manager.IsConnected())
            {
                MessageBox.Show("Cannot connect to server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DataSet temp;
            if (_manager.TryToLogIn(txtUser.Text, txtPassword.Password, out temp) && temp != null && temp.Tables.Count >= 1)
            {
                GlobalSettings.ID = Convert.ToInt32(temp.Tables[0].Rows[0][0]);
                GlobalSettings.RandomVocabulaySize = Convert.ToInt32(temp.Tables[0].Rows[0][3]);
                GlobalSettings.Time = Convert.ToInt32(temp.Tables[0].Rows[0][5]);
                GlobalSettings.SoundState = Convert.ToBoolean(temp.Tables[0].Rows[0][4]);
                this.Close();
                return;
            }
            MessageBox.Show("Invalid user or password.", "Cannot sing in.", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

