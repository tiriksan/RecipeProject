using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Model.Types
{
    public class Commerce
    {
        [JsonProperty("buys")]
        public Buy GetBuy { get; set; }

        [JsonProperty("sells")]
        public Sell GetSell { get; set; }
    }

    public class Buy
    {
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("unit_price")]
        public int UnitPrice { get; set; }
    }

    public class Sell
    {

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("unit_price")]
        public int UnitPrice { get; set; }

    }
}
