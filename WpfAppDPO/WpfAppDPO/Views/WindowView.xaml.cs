using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
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

        int curretnDamage = 0;
        int currentBlocked = 0;

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
            curretnDamage = damage;
            DamageLabel.Content = (curretnDamage == 0) ? " 0" : $"{curretnDamage:# ### ###}";
        }

        private void Blocked(int damage, int blocked)
        {
            currentBlocked = blocked;
            BlockedLabel.Content = (currentBlocked == 0) ? " 0" : $"{currentBlocked:# ### ###}";
        }
    }
}
