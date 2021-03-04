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
            EnterGame.IsEnabled = false;
            userkey.IsEnabled = false;

            dataToSend["token"] = userkey.Password;
            
            forbidden = true; //(Time < currentTime) ? true : false;

            string json = Encoding.UTF8.GetString(webClient.UploadValues("http://" + $"{Variables.UriSite}" + "/auth/desktop/?token=" + userkey.Password, dataToSend)).Replace("[","").Replace("]","");

            if (json.Contains("error"))
            {
                Variables.TokenValid = false;
                Variables.response2 = JsonConvert.DeserializeObject<AnswerServer>(json);
            }
            else
            {
                Variables.TokenValid = true;
                Variables.response = JsonConvert.DeserializeObject<Root>(json);

                EnterGame.Content = Variables.response.User.wg_nickname.Replace($"_{Variables.response.User.wg_region.ToUpper()}", " ");
                labelVersion.Content = "ver. " + Variables.response.Version.version;
            }

            if (Variables.TokenValid && Variables.VersionBuild == Variables.response.Version.version)
            {
                try
                {
                    if (Variables.TokenValid && Variables.response.User.auth_desktop_token == userkey.Password)
                    {                        
                        MessageBox.Show($"Welcome {Variables.response.User.wg_nickname}", $"Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                        ME.GetBaseAddress();
                        WindowView taskWindow = new WindowView();
                        //taskWindow.Owner = this;
                        taskWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show($"Token not valide", $"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        EnterGame.IsEnabled = true;
                        userkey.IsEnabled = true;
                        EnterGame.Content = "ВКЛЮЧИТЬ";
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show($"Игру запусти а потом оверлей\n\nException: {exc.Message}", $"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    EnterGame.IsEnabled = true;
                    userkey.IsEnabled = true;
                    EnterGame.Content = "ВКЛЮЧИТЬ";
                }
            }
            else
            {
                if (Variables.TokenValid && Variables.response.User.auth_desktop_token == userkey.Password)
                {
                    MessageBox.Show($"Версия приложения устарела, скачайте обновленную версию приложения.\n\nВерсия приложения: {Variables.VersionBuild}\nАктуальная версия: {Variables.response.Version.version}", "Приложение устарело", MessageBoxButton.OK, MessageBoxImage.Information);
                    EnterGame.IsEnabled = true;
                    userkey.IsEnabled = true;
                    EnterGame.Content = "ВКЛЮЧИТЬ";
                }
                else
                {
                    MessageBox.Show($"Token not valide", $"Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    EnterGame.IsEnabled = true;
                    userkey.IsEnabled = true;
                    EnterGame.Content = "ВКЛЮЧИТЬ";
                }
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
