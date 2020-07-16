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

            ME.GetBaseAddress();
            ME.GetProcessByName();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ME.RegisterHandler(new MemoryEditor.MemoryEditorStateHandler(Damage));
            ME.RegisterHandler(new MemoryEditor.MemoryEditorStateHandler(Blocked));

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += ME.DamageDone;
            dt.Tick += ME.DamageBlocked;
            dt.Start();
            //userNameLabel.Content = "Trial version";
        }

        private void Damage(int damage, int blocked)
        {
            int currentDamage = 0;

            if (currentDamage < damage)
            {
                currentDamage = damage;
            }

            DamageLabel.Content = (currentDamage == 0) ? " 0" : $"{currentDamage:# ### ###}";
        }

        private void Blocked(int damage, int blocked)
        {
            BlockedLabel.Content = (blocked == 0) ? " 0" : $"{blocked:# ### ###}";
        }
    }
}
