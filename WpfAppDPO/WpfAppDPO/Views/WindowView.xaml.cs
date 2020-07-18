using System;
using System.Windows;
using System.Windows.Threading;
using WpfAppDPO.Models;

namespace WpfAppDPO.Views
{
    /// <summary>
    /// Логика взаимодействия для WindowView.xaml
    /// </summary>
    public partial class WindowView : Window
    {
        MemoryEditor ME = new MemoryEditor("wotblitz.exe");

        public WindowView()
        {
            InitializeComponent();

            MaximizeButton.Click += (s, e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            try
            {
                ME.GetBaseAddress();
            }
            catch (Exception exc)
            {
                MessageBox.Show("Игру запусти а потом оверлей", $"{exc.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ME.GetProcessByName();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ME.RegisterHandler(new MemoryEditor.MemoryEditorStateHandler(DamageBlocked));

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += ME.DamageBlocked;
            dt.Start();
        }

        public void DamageBlocked(int damage, int blocked)
        {
            BlockedLabel.Content = (blocked == 0) ? " 0" : $"{blocked:# ### ###}";
            DamageLabel.Content = (damage == 0) ? " 0" : $"{damage:# ### ###}";
        }
    }
}
