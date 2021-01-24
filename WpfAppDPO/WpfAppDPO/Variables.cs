using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppDPO.Models;

namespace WpfAppDPO
{
    public static class Variables
    {
        public static Double sliderValueY = 210;
        public static Double sliderValueX = 10;

        public static List<int> damageList = new List<int>();
        public static List<int> blockedList = new List<int>();
        public static int maxDamage = 0;
        public static int maxBlocked = 0;
        public static bool DamageNotZero = false;
        public static bool onDamag = false;
        public static bool onCheat = false;
        public static int countSend = 0;
        public static string VersionBuild = "1.6.6.1";
        public static bool TokenValid = false;
        public static string UriSite = "bestofblitz.noilty.com";

        public static Root response;
        public static AnswerServer response2;
    }
}
