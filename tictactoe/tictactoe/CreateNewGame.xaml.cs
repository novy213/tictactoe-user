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
using tictactoe.models;

namespace tictactoe
{
    /// <summary>
    /// Interaction logic for CreateNewGame.xaml
    /// </summary>
    public partial class CreateNewGame : Window
    {
        public User invitedUser = new User();

        public CreateNewGame()
        {
            InitializeComponent();
        }

        private void InvitePlayer_click(object sender, RoutedEventArgs e)
        {
            InvitePlayer invitePlayer = new InvitePlayer { Owner = this };
            if (invitePlayer.ShowDialog() == true)
            {
                invitedUser = invitePlayer.PlayerListView.SelectedItem as User;
                InvitedPlayer.Text = invitedUser.Name + " " + invitedUser.Last_name;
                CreateGameIsPassword.Visibility = Visibility.Collapsed;
                CreateGamePassword.Visibility = Visibility.Collapsed;
                CreateGamePassword.Text = "";
                CreateGameIsPassword.IsChecked = false;
                IsPasswordText.Visibility = Visibility.Collapsed;
                GamePas.Visibility = Visibility.Collapsed;
            }
        }

        private void CreateGameIsPassword_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                GamePas.Visibility = Visibility.Visible;
                CreateGamePassword.Visibility = Visibility.Visible;
            }
            else
            {
                GamePas.Visibility = Visibility.Collapsed;
                CreateGamePassword.Visibility = Visibility.Collapsed;
            }
        }

        private void Create_click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
