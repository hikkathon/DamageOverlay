using System;
using System.Windows;
using WpfAppDPO.Models;
using WpfAppDPO.Views;

namespace WpfAppDPO
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        bool forbidden;
        MemoryEditor ME = new MemoryEditor("wotblitz.exe");

        Int32 Time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
        Int32 currentTime = 1597449600; // WeekTime 604800 OneWeek (https://freeonlinetools24.com/)
        string userName;

        public MainWindowView()
        {
            InitializeComponent();
            //MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            //MaximizeButton.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            //CloseButton.Click += (s, e) => Close();
            //userName = "R3ViV4L";
            //LabelCurrentTime.Content = $"Пользователь: {userName}, действительно до " + new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(currentTime) + " (+7 GTM)";
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            forbidden = (Time < currentTime) ? true : false;
            if (forbidden)
            {
                try
                {
                    ME.GetBaseAddress();                    
                    WindowView taskWindow = new WindowView();
                    taskWindow.Owner = this;
                    taskWindow.Show();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Игру запусти а потом оверлей", $"{exc.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Конец пробной версии.\rЗалетай на стрим за новой копией проги \r\rКанал Noilty\rhttps://vk.com/noiltychannel\n\nГруппа Мода https://vk.com/bestofblitz", "Время вышло",MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
