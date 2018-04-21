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

namespace ClickEnglish
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void Exit()
        {
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void KeyboardKey_Click(object sender, KeyEventArgs e)
        {
            Exit();
        }
    }
}
