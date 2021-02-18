using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

        WebClient webClient = new WebClient();
        NameValueCollection dataToSend = new NameValueCollection();

        public WindowView()
        {
            InitializeComponent();
            //ImgErr.Opacity = 0.0f;
            //ImgDone.Opacity = 0.0f;
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
            dt.Tick += LocatePanel;

            dt.Start();
        }

        // Нанесенный урон
        int Damage { get { return ME.Damage; } }

        // Заблокированный урон
        int Blocked { get { return ME.Blocked; } }

        // Жизни
        int Health { get { return ME.Health; } }

        public void DamageBlockedShow()
        {
            DamageLabel.Content = (Damage <= 0) ? " 0" : $"{Damage:# ### ###}";
            BlockedLabel.Content = (Blocked <= 0) ? " 0" : $"{Blocked:# ### ###}";
            HealthLabel.Content = (Health <= 0) ? " 0" : $"{Health:# ### ###}";

            //switch (Damage)
            //{
            //    case 0:
            //        DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
            //        break;
            //    case 1500:
            //        DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
            //        break;
            //    case 2000:
            //        DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 237, 200));
            //        break;
            //    case 2500:
            //        DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(178, 235, 242));
            //        break;
            //    case 3500:
            //        DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(225, 190, 231));
            //        break;
            //    case 5000:
            //        DamageLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 204, 165));
            //        break;
            //}

            //switch (Blocked)
            //{
            //    case 0:
            //        BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
            //        break;
            //    case 1500:
            //        BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 235, 238));
            //        break;
            //    case 2000:
            //        BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(220, 237, 200));
            //        break;
            //    case 2500:
            //        BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(178, 235, 242));
            //        break;
            //    case 3500:
            //        BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(225, 190, 231));
            //        break;
            //    case 5000:
            //        BlockedLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 204, 165));
            //        break;
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Изменение расположение панели

        public void LocatePanel(object sender, EventArgs e)
        {
            CustomLocateY = Variables.sliderValueY;
            CustomLocateX = Variables.sliderValueX;
        }

        public Double CustomLocateY
        {
            get
            {
                return _locateY;
            }
            set
            {
                _locateY = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomLocateY"));
            }
        }

        public Double CustomLocateX
        {
            get
            {
                return _locateX;
            }
            set
            {
                _locateX = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CustomLocateX"));
            }
        }

        private Double _locateY;
        private Double _locateX;

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
