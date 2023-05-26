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
using System.Windows.Markup;
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

        Thread refreshThread, waitingForOpponent, gameThread, invitesThread;
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
            refreshThread = new Thread(RefreshApp);
            refreshThread.Start();
            LoginGrid.Visibility = Visibility.Collapsed;
            GamesGrid.Visibility = Visibility.Visible;
            GetGamesResponse res = await Api.GetGamesAsync();
            GamesListView.ItemsSource = res.Games;
            waitingForOpponent = new Thread(WaitingForOpponent);
            gameThread = new Thread(GameThread);
            invitesThread = new Thread(GetInvites);
            invitesThread.Start();
        }
        private void GetInvites()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    _ = Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        GetInvitesResponse res = await Api.GetInvitesAsync();
                        if (res != null)
                        {
                            if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
                            else
                            {
                                InvitesListBox.ItemsSource = res.invites;
                                InviteButton.Content = "Invites: " + res.invites.Count.ToString();
                            }
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
        public void SetAppStateLogout()
        {
            LoginGrid.Visibility = Visibility.Visible;
            GamesGrid.Visibility = Visibility.Collapsed;
            TerminateThread(gameThread);
            TerminateThread(refreshThread);
            TerminateThread(waitingForOpponent);
            TerminateThread(invitesThread);
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
                try
                {
                    thread.Interrupt();
                    thread.Join();
                }
                catch { }
            }
        }
        private void InvitePopup_Closed(object sender, EventArgs e)
        {
            if (InviteButton != Mouse.DirectlyOver)
                InviteButton.IsChecked = false;
        }
        private async void ListViewItem_Click(object sender, MouseButtonEventArgs e)
        {
            var clickedItem = (ListViewItem)sender;
            Game data = clickedItem.Content as Game;
            if (!data.Is_password)
            {
                APIResponse res = await Api.JoinGameAsync(data.Id);
                if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
                else
                {
                    TerminateThread(gameThread);
                    TerminateThread(refreshThread);
                    TerminateThread(waitingForOpponent);
                    WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
                    GamesGrid.Visibility = Visibility.Collapsed;
                    GameGrid.Visibility = Visibility.Visible;
                    GetGameInfoResponse res2 = await Api.GetGameInfoAsync();
                    if (res2.Game.Turn == Settings.Default.user_id)
                    {
                        myTurn = true;
                        character = "X";
                        startText.Text = "You are: X";
                    }
                    else
                    {
                        myTurn = false;
                        character = "O";
                        startText.Text = "You are: O";
                    }
                    gameThread = new Thread(GameThread);
                    gameThread.Start();
                }
            }
            else
            {
                EnterPassword enterPassword = new EnterPassword { Owner = this };
                if (enterPassword.ShowDialog() == true)
                {
                    APIResponse res = await Api.JoinGameAsync(data.Id, enterPassword.Password.Text);
                    if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
                    else
                    {
                        TerminateThread(gameThread);
                        TerminateThread(refreshThread);
                        TerminateThread(waitingForOpponent);
                        WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
                        GamesGrid.Visibility = Visibility.Collapsed;
                        GameGrid.Visibility = Visibility.Visible;
                        GetGameInfoResponse res2 = await Api.GetGameInfoAsync();
                        if (res2.Game.Turn == Settings.Default.user_id)
                        {
                            myTurn = true;
                            character = "X";
                            startText.Text = "You are: X";
                        }
                        else
                        {
                            myTurn = false;
                            character = "O";
                            startText.Text = "You are: O";
                        }
                        gameThread = new Thread(GameThread);
                        gameThread.Start();
                    }
                }
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
                    TerminateThread(gameThread);
                    TerminateThread(refreshThread);
                    TerminateThread(waitingForOpponent);
                    waitingForOpponent = new Thread(WaitingForOpponent);
                    waitingForOpponent.Start();
                } 
            }
        }

        private async void AbortGame_click(object sender, RoutedEventArgs e)
        {
            TerminateThread(gameThread);
            TerminateThread(refreshThread);
            TerminateThread(waitingForOpponent);
            APIResponse res = await Api.AbortGameAsync();
            if(res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
            else
            {
                WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
                GamesGrid.Visibility = Visibility.Visible;
                GameGrid.Visibility = Visibility.Collapsed;                
                refreshThread = new Thread(RefreshApp);
                refreshThread.Start();
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
                        if (res.Game != null)
                        {
                            if (res.Game.Enemy_id != null)
                            {
                                SetGame();
                            }
                        }
                        else
                        {                            
                            TerminateThread(gameThread);
                            TerminateThread(refreshThread);
                            TerminateThread(waitingForOpponent);
                            MessageBox.Show("This game has been rejected", "Error", MessageBoxButton.OK);
                            refreshThread = new Thread(RefreshApp);
                            refreshThread.Start();
                            GameGrid.Visibility = Visibility.Collapsed;
                            WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
                            GamesGrid.Visibility = Visibility.Visible;
                            ResetGame();
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
            TerminateThread(gameThread);
            TerminateThread(refreshThread);
            TerminateThread(waitingForOpponent);
            WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
            GamesGrid.Visibility = Visibility.Collapsed;
            GameGrid.Visibility = Visibility.Visible;
            GetGameInfoResponse res2 = await Api.GetGameInfoAsync();
            if (res2.Game.Turn == Settings.Default.user_id)
            {
                myTurn = true;
                character = "X";
                startText.Text = "You are: X";
            }
            else
            {
                myTurn = false;
                character = "O";
                startText.Text = "You are: O";                
            }
            gameThread = new Thread(GameThread);
            gameThread.Start();
        }
        private async void GameThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    _ = Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        ReciveMoveResponse res = await Api.ReciveMovesAsync();
                        if (res != null)
                        {
                            if (!res.Error)
                            {
                                if (res.Moves != null && res.Moves[res.Moves.Count - 1].player_id != Settings.Default.user_id)
                                {
                                    myTurn = true;
                                }
                                UpdateGame(res);
                            }
                            if (res.Message == "Winner is O" || res.Message == "Winner is X" || res.Message == "Draw")
                            {
                                TerminateThread(gameThread);
                                TerminateThread(refreshThread);
                                TerminateThread(waitingForOpponent);                                
                                APIResponse res2 = await Api.AbortGameAsync();
                                MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
                                refreshThread = new Thread(RefreshApp);
                                refreshThread.Start();
                                GameGrid.Visibility = Visibility.Collapsed;
                                GamesGrid.Visibility = Visibility.Visible;
                                ResetGame();
                            }
                            else if (res.Message == "this game does not exist")
                            {
                                TerminateThread(gameThread);
                                TerminateThread(refreshThread);
                                TerminateThread(waitingForOpponent);
                                MessageBox.Show("Your opponent resign", "Error", MessageBoxButton.OK);
                                refreshThread = new Thread(RefreshApp);
                                refreshThread.Start();
                                GameGrid.Visibility = Visibility.Collapsed;
                                GamesGrid.Visibility = Visibility.Visible;
                                ResetGame();
                            }
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

        private async void Window_Closed(object sender, EventArgs e)
        {
            TerminateThread(gameThread);
            TerminateThread(refreshThread);
            TerminateThread(waitingForOpponent);
            TerminateThread(invitesThread);
            APIResponse res = await Api.AbortGameAsync();
        }

        private async void RejectGame_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            APIResponse res = await Api.RejectGameAsync(btn.Uid);
            if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
        }

        private async void AcceptGame_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int game_id = int.Parse(btn.Uid);
            APIResponse res = await Api.JoinGameAsync(game_id);
            if (res.Error) MessageBox.Show(res.Message, "Error", MessageBoxButton.OK);
            else
            {
                InvitePopup.IsOpen = false;
                TerminateThread(gameThread);
                TerminateThread(refreshThread);
                TerminateThread(waitingForOpponent);
                WaitingForEnemyGrid.Visibility = Visibility.Collapsed;
                GamesGrid.Visibility = Visibility.Collapsed;
                GameGrid.Visibility = Visibility.Visible;
                GetGameInfoResponse res2 = await Api.GetGameInfoAsync();
                if (res2.Game.Turn == Settings.Default.user_id)
                {
                    myTurn = true;
                    character = "X";
                    startText.Text = "You are: X";
                }
                else
                {
                    myTurn = false;
                    character = "O";
                    startText.Text = "You are: O";
                }
                gameThread = new Thread(GameThread);
                gameThread.Start();
            }
        }

        private void UpdateGame(ReciveMoveResponse res)
        {
            if (res.Moves != null)
            {
                string localCharacter = "X";
                for (int i = 0; i < res.Moves.Count; i++)
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
        private void ResetGame()
        {
            a1.Content = null;
            a2.Content = null;
            a3.Content = null;
            b1.Content = null;
            b2.Content = null;
            b3.Content = null;
            c1.Content = null;
            c2.Content = null;
            c3.Content = null;
        }
        private void RefreshApp()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    _ = Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        GetGamesResponse getGamesResponse = await Api.GetGamesAsync();
                        GamesListView.ItemsSource = getGamesResponse.Games;
                    }));
                }
            }
            catch (ThreadInterruptedException)
            {
                Debug.WriteLine("Exiting Refresh thread");
                return;
            }
        }
    }
}
