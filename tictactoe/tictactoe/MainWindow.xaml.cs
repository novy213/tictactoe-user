using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
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

        Thread refreshThread, waitingForOpponent, gameThread;
        string character;
        bool myTurn = false;
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
            waitingForOpponent = new Thread(WaitingForOpponent);
            gameThread = new Thread(GameThread);
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
            Game data = clickedItem.Content as Game;
            APIResponse res = await Api.JoinGameAsync(data.Id);
            if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
            else
            {
                WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
                GamesGrid.Visibility = Visibility.Collapsed;
                GameGrid.Visibility = Visibility.Visible;
                GetGameInfoResponse res2 = await Api.GetGameInfoAsync();
                if (res2.Game.Turn == Settings.Default.user_id)
                {
                    myTurn = true;
                    character = "X";
                }
                else
                {
                    myTurn = false;
                    character = "O";
                }
                gameThread.Start();
            }
        }

        private async void CreateNewGame_click(object sender, RoutedEventArgs e)
        {
            CreateNewGame createNewGame = new CreateNewGame { Owner = this };
            if (createNewGame.ShowDialog() == true)
            {
                APIResponse res = await Api.CreateGameAsync(createNewGame.CreateGameName.Text, createNewGame.invitedUser.Id, (bool)createNewGame.CreateGameIsPassword.IsChecked, createNewGame.CreateGamePassword.Text);
                if(res.Error) MessageBox.Show(res.Message,"Error", MessageBoxButton.OK);
                else
                {
                    WaitingForEnemyGrid.Visibility = Visibility.Visible;
                    GamesGrid.Visibility = Visibility.Collapsed;
                    waitingForOpponent.Start();
                } 
            }
        }

        private async void AbortGame_click(object sender, RoutedEventArgs e)
        {
            APIResponse res = await Api.AbortGameAsync();
            if(res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
            else
            {
                WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
                GamesGrid.Visibility = Visibility.Visible;
                GameGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void WaitingForOpponent()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    _ = Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        GetGameInfoResponse res = await Api.GetGameInfoAsync();
                        if (res.Game.Enemy_id != null)
                        {
                            SetGame();                            
                        }
                    }));
                }
            }
            catch (ThreadInterruptedException)
            {
                Debug.WriteLine("Exiting Refresh thread");
                return;
            }
        }

        private async void SendMove_click(object sender, RoutedEventArgs e)
        {
            if (myTurn)
            {
                Button btn = sender as Button;
                APIResponse res = await Api.SendMoveAsync(btn.Uid);
                if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
                else
                {
                    myTurn = false;
                }
            }
            else MessageBox.Show("This is not your turn, wait for opponent move", "Error", MessageBoxButton.OK);
        }

        private async void SetGame()
        {
            TerminateThread(waitingForOpponent);
            WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
            GamesGrid.Visibility = Visibility.Collapsed;
            GameGrid.Visibility = Visibility.Visible;
            GetGameInfoResponse res2 = await Api.GetGameInfoAsync();
            if (res2.Game.Turn == Settings.Default.user_id)
            {
                myTurn = true;
                character = "X";
            }
            else
            {
                myTurn = false;
                character = "O";
            }
            gameThread.Start();
        }
        private async void GameThread()
        {
            ReciveMoveResponse res = await Api.ReciveMovesAsync();
            if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
            else
            {
                if (res.Moves[res.Moves.Count-1].player_id != Settings.Default.user_id)
                {
                    myTurn=true;
                }
                UpdateGame(res);
            }
        }
        private void UpdateGame(ReciveMoveResponse res)
        {
            string localCharacter="X";
            for(int i = 0; i < res.Moves.Count; i++)
            {
                switch (res.Moves[i].move)
                {
                    case "a1":
                        a1.Content = localCharacter;
                        break;
                    case "a2":
                        a2.Content = localCharacter;
                        break;
                    case "a3":
                        a3.Content = localCharacter;
                        break;
                    case "b1":
                        b1.Content = localCharacter;
                        break;
                    case "b2":
                        b2.Content = localCharacter;
                        break;
                    case "b3":
                        b3.Content = localCharacter;
                        break;
                    case "c1":
                        c1.Content = localCharacter;
                        break;
                    case "c2":
                        c2.Content = localCharacter;
                        break;
                    case "c3":
                        c3.Content = localCharacter;
                        break;
                }
                if (localCharacter == "X") localCharacter = "O";
                else if (localCharacter == "O") localCharacter = "X";
            }
        }
    }
}
