using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppDPO.Models
{
    namespace CS
    {
        public class CalculateStats
        {
            public double WinRate(double battle, double wins)
            {
                return Math.Round((wins / battle) * 100, 2);
            }

            // Средний урон
            public int AverageDamage(int damage_dealt, int battle)
            {
                return damage_dealt / battle;
            }

            public string PlayerQualityWinRate(int value)
            {
                if (value >= 70)
                {
                    return "#9B87EF";
                }
                else if (value >= 60)
                {
                    return "#44A9F1";
                }
                else if (value >= 50)
                {
                    return "#C6E3A8";
                }
                else
                {
                    return "#FFFFFF";
                }
            }

            // Сокращение чисел
            public string GetSuffixValue(float value)
            {
                int zero = 0;

                while (value >= 1000)
                {
                    ++zero;

                    value /= 1000;
                }

                string suffix = string.Empty;

                switch (zero)
                {
                    case 1: suffix = "k"; break;
                    case 2: suffix = "M"; break;
                    case 3: suffix = "B"; break;
                    case 4: suffix = "T"; break;
                    case 5: suffix = "Qd"; break;
                    case 6: suffix = "Qn"; break;
                    case 7: suffix = "Sx"; break;
                    case 8: suffix = "Sp"; break;
                    case 9: suffix = "Oc"; break;
                }

                return $"{value:0.#}{suffix}";
            }
        }
    }
}
