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
    /// Interaction logic for InvitePlayer.xaml
    /// </summary>
    public partial class InvitePlayer : Window
    {
        public InvitePlayer()
        {
            InitializeComponent();
            SetUsers();
        }
        public async void SetUsers()
        {
            GetUsersResponse res = await Api.GetUsersAsync();
            PlayerListView.ItemsSource = res.Users;
            ((CollectionView)CollectionViewSource.GetDefaultView(PlayerListView.ItemsSource)).Filter = NameFilter;
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(PlayerListView.ItemsSource).Refresh();
        }
        private bool NameFilter(object item)
        {
            if (String.IsNullOrEmpty(Filter.Text))
            {
                return true;
            }
            else
            {
                var dictation = item as User;
                return
                    dictation.Name.ToLower().Contains(Filter.Text.ToLower())
                    || dictation.Last_name.ToLower().Contains(Filter.Text.ToLower());
            }
        }

        private void Invite_click(object sender, RoutedEventArgs e)
        {
            if (PlayerListView.SelectedItem != null)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Select player", "Error", MessageBoxButton.OK);
            }
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
