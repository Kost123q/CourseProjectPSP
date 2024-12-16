using EngineLibrary.EngineComponents;
using GameLibrary.Game;
using GameLibrary.Maze;
using NetworkLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameApplication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private RenderingApplication application;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public GameWindow()
        {
            InitializeComponent();

            application = new RenderingApplication();
            GameEvents.ChangeCoins += ChangeCoins;
            GameEvents.EndGame += EndGame;
            GameEvents.ChangeEffect += ChangeEffect;
            GameEvents.ChangeGun += ChangeGun;

            PreviewKeyDown += GameWindowPreviewKeyDown;
        }

        /// <summary>
        /// Обработка клавиш навигаций в окне
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        static void GameWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Событие закрытия окна
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(application != null)
                application.Dispose();
        }

        /// <summary>
        /// Соыбтия нажатия кнопки OneScreenButtonMenu
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void OneScreenButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Visibility = Visibility.Hidden;
            OneScreenGamePanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Соыбтия нажатия кнопки LANButtonMenu
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void LANButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Visibility = Visibility.Hidden;
            LANGamePanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Соыбтия нажатия кнопки LANCreateGameButtonPlay
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void LANCreateGameButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            LANButtonsPanel.Visibility = Visibility.Hidden;
            WaitConnectionText.Visibility = Visibility.Visible;

            var seed = new Random().Next();
            var server = new Server();
            server.OnDataGot += (o) => OnClientServed(server, seed);
            server.UpdateData(seed);
        }

        /// <summary>
        /// Событие обслуживания подключений сервером
        /// </summary>
        /// <param name="handler">Интернет обработчик</param>
        /// <param name="seed">Сид генерации</param>
        private void OnClientServed(INetworkHandler handler, int seed)
        {
            handler.ClearListeners();
            StartGame(new MazeScene(handler, seed, "Blue Player", "Red Player"));
        }

        /// <summary>
        /// Соыбтия нажатия кнопки LANConnectGameButtonPlay
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void LANConnectGameButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            LANButtonsPanel.Visibility = Visibility.Hidden;
            EnterIpPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Соыбтия нажатия кнопки LANConnectGameButtonEnterIp
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void LANConnectGameButtonEnterIp_Click(object sender, RoutedEventArgs e)
        {
            EnterIpPanel.Visibility = Visibility.Hidden;
            WaitConnectionText.Visibility = Visibility.Visible;

            var address = IpInput.Text;
            Client client = new Client(address);
            client.OnDataGot += (o) => OnServerResponsed(client, o);
            client.GetData<int>();
        }

        /// <summary>
        /// Событие результат подключения клиента
        /// </summary>
        /// <param name="handler">Интернет обработчик</param>
        /// <param name="obj">Полученные данные</param>
        private void OnServerResponsed(INetworkHandler handler, object obj)
        {
            var seed = (int) obj;
            handler.ClearListeners();
            StartGame(new MazeScene(handler, seed, "Red Player", "Blue Player"));
        }

        /// <summary>
        /// Соыбтия нажатия кнопки LANConnectGameButtonPlay
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void OneScreenButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            StartGame(new MazeScene());
        }

        /// <summary>
        /// Запуск игры
        /// </summary>
        /// <param name="scene">Сцена игрового приложения</param>
        private void StartGame(MazeScene scene)
        {
            MainScreen.Visibility = Visibility.Hidden;
            OneScreenGamePanel.Visibility = Visibility.Hidden;
            LANGamePanel.Visibility = Visibility.Hidden;

            BluePlayerPanel.Visibility = Visibility.Visible;
            RedPlayerPanel.Visibility = Visibility.Visible;

            formhost.Child = application.RenderForm;
            application.SetScene(scene);
            application.Run();
        }

        /// <summary>
        /// Окончание игры
        /// </summary>
        /// <param name="winPlayer">Тег игрока</param>
        private void EndGame(string winPlayer)
        {
            formhost.Visibility = Visibility.Hidden;

            BPGun.Visibility = Visibility.Hidden;
            RPGun.Visibility = Visibility.Hidden;
            BPEffectText.Text = "";
            RPEffectText.Text = "";

            if (winPlayer == "Blue Player")
                WinPlayerText.Foreground = new SolidColorBrush(Color.FromRgb(9, 46, 168));
            else
                WinPlayerText.Foreground = new SolidColorBrush(Color.FromRgb(179, 22, 22));

            WinPlayerText.Text = winPlayer + " Win!";

            Uri resourceLocater = new Uri("/Images/"+ winPlayer + ".png", UriKind.Relative);
            BitmapImage bitmap = new BitmapImage(resourceLocater);

            WinPlayerImage.Source = bitmap;

            WinPanel.Visibility = Visibility.Visible;

            GameEvents.EndGame -= EndGame;
            GameEvents.ChangeCoins -= ChangeCoins;
        }

        /// <summary>
        /// Событие смены количества монет игрока
        /// </summary>
        /// <param name="player">Тег игрока</param>
        /// <param name="value">Значение монет</param>
        private void ChangeCoins(string player, int value)
        {
            if (player == "Blue Player")
                BluePlayerCoins.Text = value.ToString();
            else
                RedPlayerCoins.Text = value.ToString();
        }

        /// <summary>
        /// Событие смены эффекта на игроке
        /// </summary>
        /// <param name="player">Тег игрока</param>
        /// <param name="effect">Накладываемый эффект</param>
        private void ChangeEffect(string player, string effect)
        {
            TextBlock textBlock;

            if (player == "Blue Player")
                textBlock = BPEffectText;
            else
                textBlock = RPEffectText;

            switch (effect)
            {
                case "Death":
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(246, 242, 0));
                    break;
                case "Slowdown":
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(29, 216, 0));
                    break;
                case "Frezze":
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 230, 255));
                    break;
            }

            textBlock.Text = effect;
        }

        /// <summary>
        /// Событие смены оружия игрока
        /// </summary>
        /// <param name="player">Тег игрока</param>
        /// <param name="gun">Добавляемое оружие</param>
        private void ChangeGun(string player, string gun)
        {
            Image image;

            if (player == "Blue Player")
                image = BPGun;
            else
                image = RPGun;

            if (gun == "")
            {
                image.Visibility = Visibility.Hidden;
            }
            else
            {
                Uri resourceLocater = new Uri("/Images/Guns/" + gun + ".png", UriKind.Relative);
                BitmapImage bitmap = new BitmapImage(resourceLocater);

                image.Source = bitmap;
                image.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Событие смены фокуса в окне рендеринга
        /// </summary>
        /// <param name="sender">Объект-отпарвитель</param>
        /// <param name="e">Аргументы</param>
        private void formhost_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            application.IsFocused = (bool)e.NewValue;
        }
    }
}
