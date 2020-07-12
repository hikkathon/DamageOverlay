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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppDPO.Views;

namespace WpfAppDPO
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        bool forbidden;

        Int32 Time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
        Int32 currentTime = 1595515722; // WeekTime 604800 OneWeek
        string userName;

        public MainWindowView()
        {
            InitializeComponent();
            MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            MaximizeButton.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            CloseButton.Click += (s, e) => Close();
            userName = "Фолловер: nikdrozd_off";
            LabelCurrentTime.Content = $"Пользователь: {userName}, действительно до " + new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(currentTime) + " (+7 GTM)";
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            forbidden = (Time < currentTime) ? true : false;
            if (forbidden)
            {
                WindowView taskWindow = new WindowView();
                taskWindow.Owner = this;
                taskWindow.Show();
            }
            else
            {
                MessageBox.Show("Конец пробной версии.\rЗалетай на стрим за новой копией проги \r\rКанал Noilty\rhttps://vk.com/noiltychannel", "Время вышло",MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
