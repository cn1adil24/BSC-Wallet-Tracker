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
    public class BscScanTokenAPI : BaseAPI
    {
        private string APIKey;
        public string ResourcesAPIKeyList => Properties.Resources.BscScanAPIKey;

        public BscScanTokenAPI()
        {
            BaseURL = ConfigurationManager.AppSettings[Localizable.LocalizedStrings.BscScanTokenAPI];
            SetAPIKey();
        }

        public async Task ExtractTokenBalance(List<TokenInfo> tokenInfo, string walletAddress)
        {
            if (RequestCounter != 0 && RequestCounter % 5 == 0)
                SetAPIKey();

            foreach (var token in tokenInfo)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(token.Contract))
                        continue;

                    var URL = string.Format(BaseURL, token.Contract, walletAddress, APIKey);
                    var response = await Client.GetFromJsonAsync<BscScanTokenPriceResponse>(URL);
                    SetTokenQuantity(token, response);
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
                catch (TimeoutException)
                {
                    // do nothing
                }
                finally
                {
                    RequestCounter++;
                }
            }

            
        }

        private void SetTokenQuantity(TokenInfo token, BscScanTokenPriceResponse response)
        {
            lock (token)
            {
                try
                {
                    var value = response.result;
                    var tokenDecimal = token.Decimal;

                    if (tokenDecimal > value.Length)
                        value = value.AppendZerosToStart(tokenDecimal - value.Length);

                    var length = value.Length;
                    var integral = value.Substring(0, length - tokenDecimal);
                    var fractional = value.Substring(length - tokenDecimal, length - integral.Length);
                    var calculatedQuantity = double.Parse(integral + "." + fractional);

                    token.Quantity = calculatedQuantity;
                }
                catch(Exception)
                {
                    // do nothing
                }
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