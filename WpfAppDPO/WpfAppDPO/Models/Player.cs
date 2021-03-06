using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppDPO.Models
{
    namespace PlayerWG
    {
        public class Meta
        {
            public int count { get; set; }
        }

        public class Data
        {
            public string nickname { get; set; }
            public int account_id { get; set; }
        }

        public class Player
        {
            public string status { get; set; }
            public Meta meta { get; set; }
            public List<Data> data { get; set; }
        }
    }
}
