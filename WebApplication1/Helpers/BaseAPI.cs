using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Web;

namespace WebApplication1.Helpers
{
    public class BaseAPI
    {
        protected HttpClient Client;
        protected string BaseURL;
        protected int RequestCounter = 0;
        public virtual bool NeedsProxy => false;

        public BaseAPI()
        {
            if (NeedsProxy)
            {
                InitializeClientWithProxy();
            }
            else
            {
                Client = new HttpClient();
            }
        }

        public void InitializeClientWithProxy()
        {
            Client = new HttpClient(GetClientHandler());
        }

        private HttpClientHandler GetClientHandler()
        {
            WebProxy proxy = new WebProxy()
            {
                Address = GetIP()
            };

            return new HttpClientHandler() { Proxy = proxy };
        }

        private Uri GetIP()
        {
            string[] proxyList = LoadProxyList();
            string url = GetRandomItemFromList(proxyList);
            url = "http://" + url;
            return new Uri(url);
        }

        private string[] LoadProxyList()
        {
            Stream proxyListStream = GetFileStreamFromResources(Localizable.LocalizedStrings.ProxyListFile);
            StreamReader stream = new StreamReader(proxyListStream);
            List<string> proxyList = new List<string>();

            // Discard the header
            string line = stream.ReadLine();

            while ((line = stream.ReadLine()) != null)
            {
                string proxy = ParseIPAddress(line);
                string port = ParsePort(line);
                proxy += ":" + port;
                proxyList.Add(proxy);
            }

            CloseStreams();
            return proxyList.ToArray();

            void CloseStreams()
            {
                proxyListStream.Close();
                stream.Close();
            }
        }

        private string ParseIPAddress(string line)
        {
            var proxy = line.Split(',')[0];
            proxy = proxy.Trim('\"');
            return proxy;
        }

        private string ParsePort(string line)
        {
            var port = line.Split(',')[9];
            port = port.Trim('\"');
            return port;
        }

        public static Stream GetFileStreamFromResources(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceStream(asm.GetName().Name + ".Properties." + resourceName);
        }

        public static string GetRandomItemFromList(string[] list)
        {
            Random rand = new Random();
            int n = rand.Next(list.Length);
            return list[n];
        }
    }
}