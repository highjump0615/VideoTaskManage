using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VideoSearch.Utils
{
    public class AppUtils
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                var client = new WebClient();
                using (client.OpenRead("https://www.baidu.com"))
                {
                    return true;
                }
            }
            catch { }

            return false;
        }
    }
}
