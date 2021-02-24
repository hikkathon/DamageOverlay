using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
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

        WebClient webClient = new WebClient();
        NameValueCollection dataToSend = new NameValueCollection();

        MemoryEditor ME = new MemoryEditor("wotblitz.exe");

        Int32 Time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
        Int32 currentTime = 1607817600; // WeekTime 604800 OneWeek (https://freeonlinetools24.com/)
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
            forbidden = true; //(Time < currentTime) ? true : false;

            try
            {
                ME.GetBaseAddress();
                WindowView taskWindow = new WindowView();
                taskWindow.Owner = this;
                EnterGame.IsEnabled = false;
                taskWindow.Show();

            }
            catch (Exception exc)
            {
                MessageBox.Show($"Игру запусти а потом оверлей\n\nException: {exc.Message}", $"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                EnterGame.IsEnabled = true;
                EnterGame.Content = "ВКЛЮЧИТЬ";
            }
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Variables.sliderValueY = slider1.Value;
        }
        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Variables.sliderValueX = slider2.Value;
        }
    }
}
