using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Tesseract;

namespace WpfAppDPO.Models
{
    class SearchTeam
    {
        String FileNameGlobal;
        public void ScanImageOrc()
        {
            Variables.nicks.Clear();
            var testImage = Environment.CurrentDirectory + $"/{FileNameGlobal}";
            try
            {
                using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImage))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();

                            //Console.WriteLine($"Recognized nickname's: {page.GetMeanConfidence() * 100}%");
                            //Console.WriteLine($"\n{text}");
                            //GB.Header = $"Screenshot team: {page.GetMeanConfidence() * 100}%";
                            string[] words = text.Replace(" ", "\n").Split('\n');
                            for (int i = 0; i < words.Length; i++)
                            {
                                if (words[i] == string.Empty) { continue; }
                                Variables.nicks.Add(words[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                //Console.WriteLine($"Error: {exc.Message}");
               //OutputText.Text = exc.Message;
            }
            finally
            {
                //taskWindow.Visibility = Visibility.Hidden;
                foreach (var nick in Variables.nicks)
                {
                    //OutputText.Text += nick + "\n";
                }
            }
        }

        public void Screenshot()
        {
            String FileName = $"BLITZBURRY {DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss")}.png";
            FileNameGlobal = FileName;

            //int screenLeft = (int)SystemParameters.VirtualScreenLeft; // Y
            //int screenTop = (int)SystemParameters.VirtualScreenTop; // X
            //int screenWidth = (int)SystemParameters.VirtualScreenWidth; // Ширина
            //int screenHeight = (int)SystemParameters.VirtualScreenHeight; // Высота

            int screenLeft = (int)(SystemParameters.VirtualScreenWidth / 2) - 510; // Ширина;
            int screenTop = (int)(SystemParameters.VirtualScreenHeight / 2) - 173; // Высота;
            int screenWidth = 1020;
            int screenHeight = 286;

            Bitmap bitmap = new Bitmap(screenWidth, screenHeight);
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(screenLeft, screenTop, 0, 0, bitmap.Size);

            bitmap.Save(FileName);

            //ImageSource image = new BitmapImage(new Uri(Environment.CurrentDirectory + $"/{FileName}", UriKind.Absolute));
            //ScanImage.Source = image;
        }
    }
}
