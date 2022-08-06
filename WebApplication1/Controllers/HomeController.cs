using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using HtmlAgilityPack;
using System.Configuration;
using System.Threading.Tasks;
using WebApplication1.Helpers;
using System;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Index(string walletAddress)
        {
            if (string.IsNullOrWhiteSpace(walletAddress))
                return View();

            List<TokenInfo> tokenInfo = new List<TokenInfo>();
            await GetTokenInfo(tokenInfo, walletAddress);
            return View(tokenInfo);
        }

        private async Task GetTokenInfo(List<TokenInfo> tokenInfo, string walletAddress)
        {
            await GetTokenList(tokenInfo, walletAddress);
            var tokenQuantity = GetTokenQuantity(tokenInfo, walletAddress);
            var tokenPrice = ExtractTokenPrice(tokenInfo);
            await Task.WhenAll(tokenQuantity, tokenPrice);
        }

        private async Task GetTokenList(List<TokenInfo> tokenInfo, string walletAddress)
        {
            BscScanAPI api = new BscScanAPI();
            await api.GetTokenList(tokenInfo, walletAddress);
        }

        private async Task GetTokenQuantity(List<TokenInfo> tokenInfo, string walletAddress)
        {
            BscScanTokenAPI api = new BscScanTokenAPI();
            await api.ExtractTokenBalance(tokenInfo, walletAddress);
        }

        private async Task ExtractTokenPrice(List<TokenInfo> tokenInfo)
        {
            PancakeSwapAPI api = new PancakeSwapAPI();
            await api.GetTokenPrices(tokenInfo);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}