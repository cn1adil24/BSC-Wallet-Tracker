using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class BscScanAPI : BaseAPI
    {
        private string APIKey;
        public string ResourcesAPIKeyList => Properties.Resources.BscScanAPIKey;

        public BscScanAPI()
        {
            BaseURL = ConfigurationManager.AppSettings[Localizable.LocalizedStrings.BscScanAPI];
            SetAPIKey();
        }

        public async Task GetTokenList(List<TokenInfo> tokenInfo, string walletAddress)
        {
            if (RequestCounter != 0 && RequestCounter % 5 == 0)
                SetAPIKey();

            var URL = string.Format(BaseURL, walletAddress, APIKey);

            try
            {
                //var response = JsonSerializeHelper.Deserialize<BscScanAPIResponse>(@"C:\Users\Shehmeer\source\repos\test.json");
                var response = await Client.GetFromJsonAsync<BscScanAPIResponse>(URL);
                ExtractTokensFromResponse(tokenInfo, response, walletAddress);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine(Localizable.LocalizedStrings.ErrorOccured);
            }
            catch (NotSupportedException)
            {
                Console.WriteLine(Localizable.LocalizedStrings.ContentNotSupported);
            }
            catch (JsonException)
            {
                Console.WriteLine(Localizable.LocalizedStrings.InvalidJSON);
            }
            finally
            {
                RequestCounter++;
            }
        }

        private void ExtractTokensFromResponse(List<TokenInfo> tokenInfo, BscScanAPIResponse response, string walletAddress)
        {
            HashSet<string> tokenNames = new HashSet<string>();

            foreach (var transactionInfo in response.result)
            {
                if (!tokenNames.Contains(transactionInfo.tokenName))
                    tokenNames.Add(transactionInfo.tokenName);
                else
                    continue;

                if (transactionInfo.to != walletAddress.ToLower())
                    continue;

                tokenInfo.Add(new TokenInfo(transactionInfo.tokenName, transactionInfo.tokenSymbol, transactionInfo.tokenDecimal, transactionInfo.contractAddress));
            }
        }

        private void SetAPIKey()
        {
            string[] APIKeyList = GetAPIKeyList();
            APIKey = GetRandomItemFromList(APIKeyList);
        }

        private string[] GetAPIKeyList()
        {
            string APIListStr = ResourcesAPIKeyList;
            return APIListStr.Split(',');
        }
    }
}