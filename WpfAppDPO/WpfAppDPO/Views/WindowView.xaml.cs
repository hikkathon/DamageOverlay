using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using WpfAppDPO.Models;

namespace WpfAppDPO.Views
{
    /// <summary>
    /// Логика взаимодействия для WindowView.xaml
    /// </summary>
    public partial class WindowView : Window, INotifyPropertyChanged
    {
        MemoryEditor ME = new MemoryEditor("wotblitz.exe");

        public WindowView()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.DataContext = this;
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
            ME.RegisterHandler(new MemoryEditor.MemoryEditorStateHandler(DamageBlockedShow));

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.Tick += ME.DamageBlocked;
            dt.Tick += WindowSize;
            dt.Tick += Counter;
            dt.Start();
        }

        int damageMax { get { return ME.Damage; } set { value = ME.Damage; } }
        int blockedMax { get { return ME.Blocked; } set { value = ME.Blocked; } }
        int damageCurrent = 0;
        int blockedCurrent = 0;
        public void Counter(object sender, EventArgs e)
        {
            if (damageCurrent < damageMax)
            {
                damageCurrent++;
                Labe1.Content = damageCurrent;
            }
            if (blockedCurrent < blockedMax)
            {
                blockedCurrent++;
                Labe2.Content = blockedCurrent;
            }
        }

        public void DamageBlockedShow()
        {
            //string block1 = (ME.Blocked == 0) ? " 0" : $"{ME.Blocked:# ### ###}";
            //string block2 = (blockedCurrent == 0) ? "0" : $"{blockedCurrent:# ### ###}";

            //string damage1 = (ME.Damage == 0) ? " 0" : $"{ME.Damage:# ### ###}";
            //string damage2 = (damageCurrent == 0) ? "0" : $"{damageCurrent:# ### ###}";

            string damage = string.Empty;
            string blocked = string.Empty;
            if(ME.Damage == 0)
            {
                damage = "0";
                DamageLabel.Content = (damageCurrent == 0) ? "0" : $"{damage:# ### ###}";
            }
            else
            {
                damage = (damageCurrent == 0) ? "0" : $"{damageCurrent:# ### ###}";
                DamageLabel.Content = (damageCurrent == 0) ? "0" : $"{damage:# ### ###}";
            }

            if (ME.Blocked == 0)
            {
                blocked = "0";
                BlockedLabel.Content = (blockedCurrent == 0) ? "0" : $"{blocked:# ### ###}";
            }
            else
            {
                blocked = (damageCurrent == 0) ? "0" : $"{blocked:# ### ###}";
                BlockedLabel.Content = (blockedCurrent == 0) ? "0" : $"{blocked:# ### ###}";
            }            
        }

        // Изменение размера окна
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void WindowSize(object sender, EventArgs e)
        {
            using (Process process = Process.GetProcessesByName("wotblitz")[0])
            {
                uint handle = (uint)process.MainWindowHandle; // хендл окна
                Rect r = new Rect();
                if (GetWindowRect((IntPtr)handle, ref r))
                {
                    CustomWidth = r.right - r.left;
                    CustomHeight = r.bottom - r.top;
                    CustomTop = r.top;
                    CustomLeft = r.left;
                }
            }
        }

        public int CustomHeight
        {
            get 
            { 
                return _height; 
            }
            set 
            {
                _height = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomHeight"));
            }
        }
        public int CustomWidth
        {
            get 
            { 
                return _width; 
            }
            set
            { 
                _width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomWidth"));
            }
        }
        public int CustomTop
        {
            get 
            { 
                return _posX; 
            }
            set 
            { 
                _posX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomTop"));
            }
        }
        public int CustomLeft
        {
            get 
            {
                return _PosY; 
            }
            set 
            { 
                _PosY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomLeft"));
            }
        }
        private int _height;
        private int _width;
        private int _posX;
        private int _PosY;
    }
}
