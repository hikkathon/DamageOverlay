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
        }
    }
}
