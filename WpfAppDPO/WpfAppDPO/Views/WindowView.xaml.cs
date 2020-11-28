using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

            healthIncrement = Health;

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.Tick += ME.DamageBlocked;
            dt.Tick += WindowSize;
            dt.Tick += LocatePanel;

            dt.Start();
        }

        int Damage { get { return ME.Damage; } set { value = ME.Damage; } }
        int Blocked { get { return ME.Blocked; } set { value = ME.Blocked; } }
        int Health { get { return ME.Health; } set { value = ME.Health; } }

        int HealthMax = 0;

        int damageIncrement = 0;
        int blockedIncrement = 0;
        int healthIncrement = 0;

        public void DamageBlockedShow()
        {
            //string block1 = (ME.Blocked == 0) ? " 0" : $"{ME.Blocked:# ### ###}";
            //string block2 = (blockedCurrent == 0) ? "0" : $"{blockedCurrent:# ### ###}";

            //string damage1 = (ME.Damage == 0) ? " 0" : $"{ME.Damage:# ### ###}";
            //string damage2 = (damageCurrent == 0) ? "0" : $"{damageCurrent:# ### ###}";

            if (damageIncrement < Damage)
            {
                damageIncrement++;
            }
            if (blockedIncrement < Blocked)
            {
                blockedIncrement++;
            }

            if (Health > HealthMax)
            {
                HealthMax = Health;
            }
            else
            {
                HealthMax = 0;
            }

            if (healthIncrement < HealthMax)
            {
                healthIncrement++;
            }

            int damage = (Damage == 0) ? damageIncrement-- : damageIncrement;
            int blocked = (Blocked == 0) ? blockedIncrement-- : blockedIncrement;
            int health = (Health == 0) ? healthIncrement-- : healthIncrement;

            DamageLabel.Content = (damageIncrement <= 0) ? " 0" : $"{damage:# ### ###}";
            BlockedLabel.Content = (blockedIncrement <= 0) ? " 0" : $"{blocked:# ### ###}";
            HealthLabel.Content = (healthIncrement <= 0) ? " 0" : $"{Health:# ### ###}";

            switch (damageIncrement)
            {
                case 0:
                    DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
                    break;
                case 1500:
                    DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
                    break;
                case 2000:
                    DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 237, 200));
                    break;
                case 2500:
                    DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(178, 235, 242));
                    break;
                case 3500:
                    DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(225, 190, 231));
                    break;
                case 5000:
                    DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 204, 165));
                    break;
            }

            switch (blockedIncrement)
            {
                case 0:
                    BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
                    break;
                case 1500:
                    BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
                    break;
                case 2000:
                    BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 237, 200));
                    break;
                case 2500:
                    BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(178, 235, 242));
                    break;
                case 3500:
                    BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(225, 190, 231));
                    break;
                case 5000:
                    BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 204, 165));
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Изменение расположение панели

        public void LocatePanel(object sender, EventArgs e)
        {
            CustomLocate = Variables.sliderValue;
        }

        public Double CustomLocate
        {
            get
            {
                return _locate;
            }
            set
            {
                _locate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomLocate"));
            }
        }

        private Double _locate;

        #endregion

        #region Изменение размера окна

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

        #endregion
    }
    public class Asimut : OnPropertyChangedClass
    {
        private BitmapImage _image;
        private double _angle;
        private double _offsetImage;

        public BitmapImage Image { get => _image; set => SetProperty(ref _image, value); }
        public double Angle { get => _angle; set => SetProperty(ref _angle, value); }
        public double OffsetImage { get => _offsetImage; private set => SetProperty(ref _offsetImage, value); }

        protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string propertyName)
        {
            base.PropertyNewValue(ref fieldProperty, newValue, propertyName);
            if (propertyName == nameof(Image) || propertyName == nameof(Angle))
            {
                double angele = Angle % 360;
                if (angele > 0)
                {
                    angele -= 360;
                }

                OffsetImage = Image.Width * angele / 360;
            }
        }
    }
}
