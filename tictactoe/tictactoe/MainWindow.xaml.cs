using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tictactoe.models;
using tictactoe.Properties;

namespace tictactoe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (Settings.Default.accessToken != "" && Settings.Default.user_id != 0) SetAppStateLogin();
            else SetAppStateLogout();
        }
        public async void SetAppStateLogin()
        {
            //refreshTherad = new Thread(RefreshApp);
            //refreshTherad.Start();
            LoginGrid.Visibility = Visibility.Collapsed;
            GamesGrid.Visibility = Visibility.Visible;
            GetGamesResponse res = await Api.GetGamesAsync();
            GamesListView.ItemsSource = res.Games;
        }
        public void SetAppStateLogout()
        {
            LoginGrid.Visibility = Visibility.Visible;
            GamesGrid.Visibility = Visibility.Collapsed;
            //TerminateThread(refreshTherad);
        }
        private async void Login_click(object sender, RoutedEventArgs e)
        {
            LoginResponse res = await Api.LoginAsync(LoginName.Text, PasswordName.Text);
            if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
            else SetAppStateLogin();
        }
        private void GoToRegister_click(object sender, RoutedEventArgs e)
        {
            RegisterGrid.Visibility = Visibility.Visible;
            LoginGrid.Visibility = Visibility.Collapsed;
        }
        private async void Register_click(object sender, RoutedEventArgs e)
        {
            string login = LoginRegister.Text;
            string password = PasswordRegister.Text;
            string name = NameRegister.Text;
            string last_name = LNameRegister.Text;
            if (login == "" || password == "" || name == "" || last_name == "")
            {
                MessageBox.Show("All fields are required", "Error");
            }
            else
            {
                APIResponse res = await Api.RegisterAsync(login, password, name, last_name);
                if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
                else
                {
                    RegisterGrid.Visibility = Visibility.Collapsed;
                    LoginGrid.Visibility = Visibility.Visible;
                }
            }
        }
        private void BackRegister_click(object sender, RoutedEventArgs e)
        {
            RegisterGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
        }

        private async void Logout_click(object sender, RoutedEventArgs e)
        {
            APIResponse res = await Api.LogoutAsync();
            if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
            else SetAppStateLogout();
        }
        private void TerminateThread(Thread thread)
        {
            if (thread != null)
            {
                thread.Interrupt();
                thread.Join();
            }
        }

        private async void ListViewItem_Click(object sender, MouseButtonEventArgs e)
        {
            var clickedItem = (ListViewItem)sender;
            var data = clickedItem.Content;            
        }
    }
}
