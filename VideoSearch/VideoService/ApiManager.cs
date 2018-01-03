using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace VideoSearch.VideoService
{
    public class ApiManager
    {
        public static string API_PATH = ConfigurationManager.AppSettings["webApiUrlBase"];

        private static ApiManager _Instance;
        public static ApiManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ApiManager();

                return _Instance;
            }
        }

        /// <summary>
        /// Queries all tasks info
        /// </summary>
        /// <returns></returns>
        public XElement GetQueryTaskList()
        {
            return sendToServiceByGet(API_PATH + "/QueryTaskList");
        }

        /// <summary>
        /// Send Http Get Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public XElement sendToServiceByGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    XDocument xmlDoc = new XDocument();
                    try
                    {
                        xmlDoc = XDocument.Parse(sr.ReadToEnd());
                        return xmlDoc.Root;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
    }
}
