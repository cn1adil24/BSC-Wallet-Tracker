using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TokenInfo
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public string Contract { get; set; }
        public int Decimal { get; set; }
        public double Quantity { get; set; }

        public double Value
        {
            get
            {
                double value = -1;

                if (Price > 0)
                    value = Price * Quantity;

                return value;
            }
        }

        public TokenInfo(string Name, string Symbol, string Decimal, string Contract = null)
        {
            this.Name = Name;
            this.Symbol = Symbol;
            this.Decimal = int.Parse(Decimal);
            this.Contract = string.IsNullOrWhiteSpace(Contract) ? string.Empty : Contract;

            Price = -1;
        }
    }
}