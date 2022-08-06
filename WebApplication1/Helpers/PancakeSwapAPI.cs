using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using WebApplication1.Models;
using System.Configuration;
using System.Threading;

namespace WebApplication1.Helpers
{
    public class PancakeSwapAPI : BaseAPI
    {
        //public override bool NeedsProxy => true;

        public PancakeSwapAPI()
        {
            BaseURL = ConfigurationManager.AppSettings[Localizable.LocalizedStrings.PancakeSwapAPI];
        }

        public async Task GetTokenPrices(List<TokenInfo> tokenInfo)
        {
            foreach (var token in tokenInfo)
            {
                if (string.IsNullOrWhiteSpace(token.Contract))
                    continue;

                /*
                if (RequestCounter != 0 && RequestCounter % 5 == 0)
                    InitializeClientWithProxy();
                */

                if (RequestCounter != 0 && RequestCounter % 5 == 0)
                    Thread.Sleep(500);

                var URL = string.Format(BaseURL, token.Contract);

                try
                {
                    var response = await Client.GetFromJsonAsync<PancakeSwapAPIResponse>(URL);
                    LoadTokenInfo(token, response);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine(Localizable.LocalizedStrings.ErrorOccured, ex.Message);
                }
                catch (NotSupportedException ex)
                {
                    Console.WriteLine(Localizable.LocalizedStrings.ContentNotSupported, ex.Message);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine(Localizable.LocalizedStrings.InvalidJSON, ex.Message);
                }
                catch(TimeoutException ex)
                {
                    // do nothing
                }
                finally
                {
                    RequestCounter++;
                }
            }
        }

        private void LoadTokenInfo(TokenInfo token, PancakeSwapAPIResponse response)
        {
            lock(token)
            {
                if (token.Name == response.data.name)
                {
                    token.Price = response.data.price;
                }
            }
        }
    }
}