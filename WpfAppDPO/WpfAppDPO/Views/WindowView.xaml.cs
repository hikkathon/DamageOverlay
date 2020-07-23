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
            ME.RegisterHandler(new MemoryEditor.MemoryEditorStateHandler(DamageBlocked));

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.Tick += ME.DamageBlocked;
            dt.Tick += WindowSize;
            dt.Start();
        }

        public void DamageBlocked(int damage, int blocked)
        {
            BlockedLabel.Content = (ME.Blocked == 0) ? " 0" : $"{ME.Blocked:# ### ###}";
            DamageLabel.Content = (ME.Damage == 0) ? " 0" : $"{ME.Damage:# ### ###}";
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
