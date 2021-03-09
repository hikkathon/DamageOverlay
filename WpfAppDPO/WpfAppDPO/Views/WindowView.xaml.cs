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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAppDPO.Models;
using WpfAppDPO.Models.CS;

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
            dt.Tick += ShowPrechentage;

            dt.Start();
        }

        // Нанесенный урон
        int Damage { get { return ME.Damage; } }

        // Заблокированный урон
        int Blocked { get { return ME.Blocked; } }

        // Жизни
        int Health { get { return ME.Health; } }

        int wgID { get { return ME.WGID; } }

        public void DataSend()
        {
            try
            {
                if (Damage > 0)
                {
                    Variables.onDamag = true;
                }

                if (Variables.maxDamage > 30000 || Variables.maxBlocked > 30000)
                {
                    Variables.onCheat = true;
                    //TextBox2.Content = $"Validate false";
                    //ImgErr.Opacity = 1.0f;
                    //ImgDone.Opacity = 0.0f;
                }
                else
                {
                    Variables.onCheat = false;
                    //TextBox2.Content = $"Validate true";
                    //ImgDone.Opacity = 1.0f;
                    //ImgErr.Opacity = 0.0f;

                    Variables.damageList.Add(Damage);
                    Variables.blockedList.Add(Blocked);

                    Variables.maxDamage = Variables.damageList.Distinct().Max();
                    Variables.maxBlocked = Variables.blockedList.Distinct().Max();
                }

                if (Damage == 0 && Variables.onDamag && !Variables.onCheat)
                {
                    var rnd = new Random();
                    int val = rnd.Next(1, 6);
                    Thread.Sleep(val * 1000);

                    Variables.countSend++;
                    string json = Encoding.UTF8.GetString(webClient.UploadValues(
                        @"http://" + $"{Variables.UriSite}" + "/push/data/desktop/?user_id=" + Variables.response.User.id +
                        "&user_wg_account_id=" + Variables.response.User.wg_account_id +
                        "&user_wg_region=" + Variables.response.User.wg_region +
                        "&user_auth_desktop_token=" + Variables.response.User.auth_desktop_token +
                        "&dmg=" + Variables.maxDamage +
                        "&dmg_block=" + Variables.maxBlocked, dataToSend));

                    Variables.response2 = JsonConvert.DeserializeObject<AnswerServer>(json);

                    //TextBox1.Content += $"SENT TO SERVER #{Variables.countSend}" + "\n" + "DAMAGE: " + Variables.maxDamage + "\n" + "BLOCK: " + Variables.maxBlocked;
                    //TextBox1.Content = $"Send data: {Variables.countSend}\nStatus: {Variables.response2.status}\nMessage: {Variables.response2.message}";

                    Variables.onDamag = false;

                    Variables.damageList.Clear();
                    Variables.blockedList.Clear();
                    Variables.maxDamage = 0;
                    Variables.maxBlocked = 0;

                    GridPlayerList.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception exc)
            {
                Variables.countSend = 0;
                //TextBox1.Content = $"Send data error:  + {exc.Message}\nStatus: {Variables.response2.status}\nMessage: {Variables.response2.message}";
            }
        }

        public void DamageBlockedShow()
        {
            DamageLabel.Content = (Damage <= 0) ? " 0" : $"{Damage:# ### ###}";
            BlockedLabel.Content = (Blocked <= 0) ? " 0" : $"{Blocked:# ### ###}";
            //HealthLabel.Content = (Health <= 0) ? " 0" : $"{Health:# ### ###}";
            //TextBox1.Content = wgID.ToString();

            DataSend();

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

        public void ShowPrechentage(object sender, EventArgs e)
        {
            CalculateStats cs = new CalculateStats();

            if (Variables.players.Count > 0)
            {
                for (int i = 0; i < Variables.players.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_1.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if(wr >= 60)
                                    {
                                        Player_1.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_1.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_1.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_1.Content = $"{wr}% {Variables.accounts.ElementAt(i).data.info.nickname}";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_1.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_1.Content = "Танкист не найден";
                            }
                            break;
                        case 1:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_2.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_2.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_2.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_2.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_2.Content = $"{wr}% {Variables.accounts.ElementAt(i).data.info.nickname}";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_2.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_2.Content = "Танкист не найден";
                            }
                            break;
                        case 2:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_3.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_3.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_3.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_3.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_3.Content = $"{wr}% {Variables.accounts.ElementAt(i).data.info.nickname}";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_3.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_3.Content = "Танкист не найден";
                            }
                            break;
                        case 3:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_4.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_4.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_4.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_4.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_4.Content = $"{wr}% {Variables.accounts.ElementAt(i).data.info.nickname}";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_4.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_4.Content = "Танкист не найден";
                            }
                            break;
                        case 4:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_5.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_5.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_5.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_5.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_5.Content = $"{wr}% {Variables.accounts.ElementAt(i).data.info.nickname}";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_5.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_5.Content = "Танкист не найден";
                            }
                            break;
                        case 5:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_6.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_6.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_6.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_6.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_6.Content = $"{wr}% {Variables.accounts.ElementAt(i).data.info.nickname}";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_6.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_6.Content = "Танкист не найден";
                            }
                            break;
                        case 6:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_7.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_7.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_7.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_7.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_7.Content = $"{wr}% {Variables.accounts.ElementAt(i).data.info.nickname}";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_7.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_7.Content = "Танкист не найден";
                            }
                            break;
                        case 7:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_8.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_8.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_8.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_8.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_8.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname} {wr}%";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_8.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_8.Content = "Танкист не найден";
                            }
                            break;
                        case 8:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_9.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_9.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_9.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_9.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_9.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname} {wr}%";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_9.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_9.Content = "Танкист не найден";
                            }
                            break;
                        case 9:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_10.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_10.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_10.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_10.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_10.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname} {wr}%";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_10.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_10.Content = "Танкист не найден";
                            }
                            break;
                        case 10:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_11.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_11.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_11.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_11.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_11.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname} {wr}%";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_11.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_11.Content = "Танкист не найден";
                            }
                            break;
                        case 11:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_12.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_12.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_12.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_12.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_12.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname} {wr}%";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_12.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_12.Content = "Танкист не найден";
                            }
                            break;
                        case 12:
                            // TODO:
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_13.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_13.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_13.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_13.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_13.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname} {wr}%";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_13.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_13.Content = "Танкист не найден";
                            }
                            break;
                        case 13:
                            // TODO: 
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);

                                    if (wr >= 70)
                                    {
                                        Player_14.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_14.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_14.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_14.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_14.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname} {wr}%";
                                }
                                catch (Exception exc){}
                            }
                            else
                            {
                                Player_14.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 78, 62));
                                Player_14.Content = "Танкист не найден";
                            }
                            break;
                        default:
                            // TODO: 
                            break;
                    }
                }
            }
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

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            if (RightGrid.Visibility == Visibility.Hidden)
            {
                RightGrid.Visibility = Visibility.Visible;
            }
            else
            {
                RightGrid.Visibility = Visibility.Hidden;
            }
        }

        private void BtnCloseOverlay_Click(object sender, RoutedEventArgs e)
        {
            var p1 = Variables.nicks;
            var p2 = Variables.players;
            var p3 = Variables.accounts;
            var p4 = Variables.DefaultJson;
            Close();
        }

        WhiteWindow taskWindow = new WhiteWindow();
        SearchTeam searchTeam = new SearchTeam();
        private void BtnLoadStats_Click(object sender, RoutedEventArgs e)
        {
            GridPlayerList.Visibility = Visibility.Visible;

            taskWindow.Owner = this;
            taskWindow.Show();
            searchTeam.Screenshot();
            searchTeam.ScanImageOrc();
            taskWindow.Visibility = Visibility.Hidden;

            Task.Run(() => SearchPlayer.Search());
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (toggleButton.IsChecked == true)
            {
                GridDamagePanel.Visibility = Visibility.Visible;
            }
            else
            {
                GridDamagePanel.Visibility = Visibility.Hidden;
            }
        }

        private void TestEvent_Click(object sender, RoutedEventArgs e)
        {

        }
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
