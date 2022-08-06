using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows.Forms;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class WalletAddressScrapper
    {
        private readonly WebClient Client;
        private readonly string BaseURL;
        WebBrowser browser = new WebBrowser();

        public WalletAddressScrapper()
        {
            Client = new WebClient();
            BaseURL = ConfigurationManager.AppSettings[Localizable.LocalizedStrings.BscScanTokenList];
        }

        public void SetTokenQuantity(List<TokenInfo> tokenInfo, string walletAddress)
        {

            int pageNo = 1;
            
            browser.AllowNavigation = true;
            // optional but I use this because it stops javascript errors breaking your scraper
            browser.ScriptErrorsSuppressed = true;
            // you want to start scraping after the document is finished loading so do it in the function you pass to this handler
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowserControl_DocumentCompleted);
            browser.Navigate(string.Format(BaseURL, walletAddress, pageNo));
            /*
            while (true)
            {
                string page = GetPage(walletAddress, pageNo);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page);

                var table = LoadTables(doc);
                pageNo++;
            }*/

            //var htmlDoc = Web.Load(URL);
            //var table = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'table table-hover')]");
        }

        private void webBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection divs = browser.Document.GetElementsByTagName("div");

            foreach (HtmlElement div in divs)
            {
                //do something
            }
        }

        private static List<List<string>> LoadTables(HtmlAgilityPack.HtmlDocument doc)
        {
            List<List<string>> table = doc.DocumentNode.SelectSingleNode("//table[@class='table table-align-middle table-hover']")
                                        .Descendants("tr")
                                        //.Skip(1)
                                        .Where(tr => tr.Elements("td").Count() > 1)
                                        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                                        .ToList();
            return table;
        }

        private string GetPage(string walletAddress, int pageNo)
        {
            var URL = string.Format(BaseURL, walletAddress, pageNo);
            return Client.DownloadString(URL);
        }
    }
}