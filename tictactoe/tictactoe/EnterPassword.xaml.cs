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

namespace tictactoe
{
    /// <summary>
    /// Interaction logic for EnterPassword.xaml
    /// </summary>
    public partial class EnterPassword : Window
    {
        public EnterPassword()
        {
            InitializeComponent();
        }

        private void Join_click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;   
        }
    }
}
