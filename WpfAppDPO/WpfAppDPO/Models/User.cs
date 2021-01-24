using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppDPO.Models
{
    public class User
    {
        public int id { get; set; }
        public string wg_nickname { get; set; }
        public int wg_account_id { get; set; }
        public string wg_region { get; set; }
        public string auth_desktop_token { get; set; }
        public object email_verified_at { get; set; }
    }

    public class Version
    {
        public string version { get; set; }
    }

    public class Root
    {
        public User User { get; set; }
        public Version Version { get; set; }
    }
}
