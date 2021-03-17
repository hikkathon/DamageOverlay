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
using System.Windows.Input;
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

        // WG ID
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

                    Player_1_Nick.Content  = " ";
                    Player_2_Nick.Content  = " ";
                    Player_3_Nick.Content  = " ";
                    Player_4_Nick.Content  = " ";
                    Player_5_Nick.Content  = " ";
                    Player_6_Nick.Content  = " ";
                    Player_7_Nick.Content  = " ";
                    Player_8_Nick.Content  = " ";
                    Player_9_Nick.Content  = " ";
                    Player_10_Nick.Content = " ";
                    Player_11_Nick.Content = " ";
                    Player_12_Nick.Content = " ";
                    Player_13_Nick.Content = " ";
                    Player_14_Nick.Content = " ";
                    Player_1_WR.Content = " ";
                    Player_2_WR.Content = " ";
                    Player_3_WR.Content = " ";
                    Player_4_WR.Content = " ";
                    Player_5_WR.Content = " ";
                    Player_6_WR.Content = " ";
                    Player_7_WR.Content = " ";
                    Player_8_WR.Content = " ";
                    Player_9_WR.Content = " ";
                    Player_10_WR.Content = " ";
                    Player_11_WR.Content = " ";
                    Player_12_WR.Content = " ";
                    Player_13_WR.Content = " ";
                    Player_14_WR.Content = " ";
                    Player_1_Battle.Content = " ";
                    Player_2_Battle.Content = " ";
                    Player_3_Battle.Content = " ";
                    Player_4_Battle.Content = " ";
                    Player_5_Battle.Content = " ";
                    Player_6_Battle.Content = " ";
                    Player_7_Battle.Content = " ";
                    Player_8_Battle.Content = " ";
                    Player_9_Battle.Content = " ";
                    Player_10_Battle.Content = " ";
                    Player_11_Battle.Content = " ";
                    Player_12_Battle.Content = " ";
                    Player_13_Battle.Content = " ";
                    Player_14_Battle.Content = " ";
                    Player_1_Damage.Content = " ";
                    Player_2_Damage.Content = " ";
                    Player_3_Damage.Content = " ";
                    Player_4_Damage.Content = " ";
                    Player_5_Damage.Content = " ";
                    Player_6_Damage.Content = " ";
                    Player_7_Damage.Content = " ";
                    Player_8_Damage.Content = " ";
                    Player_9_Damage.Content = " ";
                    Player_10_Damage.Content = " ";
                    Player_11_Damage.Content = " ";
                    Player_12_Damage.Content = " ";
                    Player_13_Damage.Content = " ";
                    Player_14_Damage.Content = " ";

                    Variables.accounts.Clear();
                    Variables.players.Clear();
                    Variables.nicks.Clear();
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
            DamageText.Text = (Damage <= 0) ? " 0" : $"{Damage:# ### ###}";
            BlockedText.Text = (Blocked <= 0) ? " 0" : $"{Blocked:# ### ###}";
            HealthText.Text = (Health <= 0) ? " 0" : $"{Health:# ### ###}";
            //TextBox1.Content = wgID.ToString();

            if (Health == 0)
            {
                GridPlayerList.Visibility = Visibility.Hidden;
            }
            else
            {
                GridPlayerList.Visibility = Visibility.Visible;
            }

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
            if (Variables.players.Count > 0)
            {
                GridPlayerList.Visibility = Visibility.Visible;
                CalculateStats cs = new CalculateStats();
                for (int i = 0; i < Variables.players.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            // TODO: 
                            Player_1_LoadGif.Visibility = Visibility.Visible;
                            Player_1_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_1_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_1_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_1_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_1_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_1_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_1_WR.Content = $"{wr}%";
                                    Player_1_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_1_Damage.Content = $" {ad}";
                                    Player_1_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_1_Nick.Content = "-";
                                Player_1_WR.Content = "--.--%";
                                Player_1_Battle.Content = "-";
                                Player_1_Damage.Content = "-";
                                Player_1_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 1:
                            // TODO: 
                            Player_2_LoadGif.Visibility = Visibility.Visible;
                            Player_2_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_2_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_2_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_2_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_2_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_2_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_2_WR.Content = $"{wr}%";
                                    Player_2_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_2_Damage.Content = $" {ad}";
                                    Player_2_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_2_Nick.Content = "-";
                                Player_2_WR.Content = "--.--%";
                                Player_2_Battle.Content = "-";
                                Player_2_Damage.Content = "-";
                                Player_2_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 2:
                            // TODO: 
                            Player_3_LoadGif.Visibility = Visibility.Visible;
                            Player_3_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_3_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_3_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_3_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_3_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_3_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_3_WR.Content = $"{wr}%";
                                    Player_3_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_3_Damage.Content = $" {ad}";
                                    Player_3_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_3_Nick.Content = "-";
                                Player_3_WR.Content = "--.--%";
                                Player_3_Battle.Content = "-";
                                Player_3_Damage.Content = "-";
                                Player_3_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 3:
                            // TODO: 
                            Player_4_LoadGif.Visibility = Visibility.Visible;
                            Player_4_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_4_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_4_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_4_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_4_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_4_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_4_WR.Content = $"{wr}%";
                                    Player_4_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_4_Damage.Content = $" {ad}";
                                    Player_4_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_4_Nick.Content = "-";
                                Player_4_WR.Content = "--.--%";
                                Player_4_Battle.Content = "-";
                                Player_4_Damage.Content = "-";
                                Player_4_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 4:
                            // TODO: 
                            Player_5_LoadGif.Visibility = Visibility.Visible;
                            Player_5_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_5_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_5_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_5_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_5_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_5_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_5_WR.Content = $"{wr}%";
                                    Player_5_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_5_Damage.Content = $" {ad}";
                                    Player_5_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_5_Nick.Content = "-";
                                Player_5_WR.Content = "--.--%";
                                Player_5_Battle.Content = "-";
                                Player_5_Damage.Content = "-";
                                Player_5_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 5:
                            // TODO: 
                            Player_6_LoadGif.Visibility = Visibility.Visible;
                            Player_6_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_6_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_6_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_6_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_6_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_6_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_6_WR.Content = $"{wr}%";
                                    Player_6_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_6_Damage.Content = $" {ad}";
                                    Player_6_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_6_Nick.Content = "-";
                                Player_6_WR.Content = "--.--%";
                                Player_6_Battle.Content = "-";
                                Player_6_Damage.Content = "-";
                                Player_6_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 6:
                            // TODO: 
                            Player_7_LoadGif.Visibility = Visibility.Visible;
                            Player_7_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_7_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_7_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_7_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_7_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_7_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_7_WR.Content = $"{wr}%";
                                    Player_7_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_7_Damage.Content = $" {ad}";
                                    Player_7_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_7_Nick.Content = "-";
                                Player_7_WR.Content = "--.--%";
                                Player_7_Battle.Content = "-";
                                Player_7_Damage.Content = "-";
                                Player_7_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 7:
                            // TODO: 
                            Player_8_LoadGif.Visibility = Visibility.Visible;
                            Player_8_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_8_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_8_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_8_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_8_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_8_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_8_WR.Content = $"{wr}%";
                                    Player_8_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_8_Damage.Content = $" {ad}";
                                    Player_8_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_8_Nick.Content = "-";
                                Player_8_WR.Content = "--.--%";
                                Player_8_Battle.Content = "-";
                                Player_8_Damage.Content = "-";
                                Player_8_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 8:
                            // TODO: 
                            Player_9_LoadGif.Visibility = Visibility.Visible;
                            Player_9_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_9_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_9_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_9_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_9_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_9_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_9_WR.Content = $"{wr}%";
                                    Player_9_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_9_Damage.Content = $" {ad}";
                                    Player_9_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_9_Nick.Content = "-";
                                Player_9_WR.Content = "--.--%";
                                Player_9_Battle.Content = "-";
                                Player_9_Damage.Content = "-";
                                Player_9_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 9:
                            // TODO: 
                            Player_10_LoadGif.Visibility = Visibility.Visible;
                            Player_10_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_10_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_10_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_10_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_10_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_10_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_10_WR.Content = $"{wr}%";
                                    Player_10_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_10_Damage.Content = $" {ad}";
                                    Player_10_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_10_Nick.Content = "-";
                                Player_10_WR.Content = "--.--%";
                                Player_10_Battle.Content = "-";
                                Player_10_Damage.Content = "-";
                                Player_10_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 10:
                            // TODO: 
                            Player_11_LoadGif.Visibility = Visibility.Visible;
                            Player_11_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_11_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_11_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_11_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_11_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_11_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_11_WR.Content = $"{wr}%";
                                    Player_11_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_11_Damage.Content = $" {ad}";
                                    Player_11_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_11_Nick.Content = "-";
                                Player_11_WR.Content = "--.--%";
                                Player_11_Battle.Content = "-";
                                Player_11_Damage.Content = "-";
                                Player_11_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 11:
                            // TODO: 
                            Player_12_LoadGif.Visibility = Visibility.Visible;
                            Player_12_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_12_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_12_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_12_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_12_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_12_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_12_WR.Content = $"{wr}%";
                                    Player_12_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_12_Damage.Content = $" {ad}";
                                    Player_12_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_12_Nick.Content = "-";
                                Player_12_WR.Content = "--.--%";
                                Player_12_Battle.Content = "-";
                                Player_12_Damage.Content = "-";
                                Player_12_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 12:
                            // TODO:
                            Player_13_LoadGif.Visibility = Visibility.Visible;
                            Player_13_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_13_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_13_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_13_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_13_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_13_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_13_WR.Content = $"{wr}%";
                                    Player_13_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_13_Damage.Content = $" {ad}";
                                    Player_13_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_13_Nick.Content = "-";
                                Player_13_WR.Content = "--.--%";
                                Player_13_Battle.Content = "-";
                                Player_13_Damage.Content = "-";
                                Player_13_LoadGif.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 13:
                            // TODO: 
                            Player_14_LoadGif.Visibility = Visibility.Visible;
                            Player_14_Nick.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            if (Variables.players.ElementAt(i).status == "ok" && Variables.players.ElementAt(i).data.Count > 0)
                            {
                                try
                                {
                                    double wr = cs.WinRate((float)Variables.accounts.ElementAt(i).data.info.statistics.all.battles, (float)Variables.accounts.ElementAt(i).data.info.statistics.all.wins);
                                    int ad = cs.AverageDamage(Variables.accounts.ElementAt(i).data.info.statistics.all.damage_dealt, Variables.accounts.ElementAt(i).data.info.statistics.all.battles);

                                    if (wr >= 70)
                                    {
                                        Player_14_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 135, 239));
                                    }
                                    else if (wr >= 60)
                                    {
                                        Player_14_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 169, 241));
                                    }
                                    else if (wr >= 50)
                                    {
                                        Player_14_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 227, 168));
                                    }
                                    else
                                    {
                                        Player_14_WR.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                                    }

                                    Player_14_Nick.Content = $"{Variables.accounts.ElementAt(i).data.info.nickname}";
                                    Player_14_WR.Content = $"{wr}%";
                                    Player_14_Battle.Content = $" {cs.GetSuffixValue(Variables.accounts.ElementAt(i).data.info.statistics.all.battles)}";
                                    Player_14_Damage.Content = $" {ad}";
                                    Player_14_LoadGif.Visibility = Visibility.Hidden;
                                }
                                catch (Exception exc) { }
                            }
                            else
                            {
                                Player_14_Nick.Content = "-";
                                Player_14_WR.Content = "--.--%";
                                Player_14_Battle.Content = "-";
                                Player_14_Damage.Content = "-";
                                Player_14_LoadGif.Visibility = Visibility.Hidden;
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
            var p5 = Variables.test_temp;
            Close();
        }

        WhiteWindow taskWindow = new WhiteWindow();
        SearchTeam searchTeam = new SearchTeam();
        private void BtnLoadStats_Click(object sender, RoutedEventArgs e)
        {
            taskWindow.Owner = this;
            taskWindow.Show();
            searchTeam.Screenshot();
            searchTeam.ScanImageOrc();
            taskWindow.Visibility = Visibility.Hidden;
            Task.Run(() => SearchPlayer.Search());
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (toggleButtonDamage.IsChecked == true)
            {
                GridDamage.Visibility = Visibility.Visible;
            }
            else
            {
                GridDamage.Visibility = Visibility.Hidden;
            }

            if (toggleButtonBlocked.IsChecked == true)
            {
                GridBlocked.Visibility = Visibility.Visible;
            }
            else
            {
                GridBlocked.Visibility = Visibility.Hidden;
            }

            if (toggleButtonHealth.IsChecked == true)
            {
                GridHealth.Visibility = Visibility.Visible;
            }
            else
            {
                GridHealth.Visibility = Visibility.Hidden;
            }
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
