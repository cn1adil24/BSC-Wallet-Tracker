namespace WebApplication1.Models
{
    public class PancakeSwapAPIResponse
    {
        public long updated_at { get; set; }
        public TokenData data { get; set; }

        public class TokenData
        {
            public string name { get; set; }
            public string symbol { get; set; }
            public double price { get; set; }
            public double price_BNB { get; set; }
        }
    }
}