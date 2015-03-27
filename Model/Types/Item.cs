using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Model.Types
{
    public class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("flags")]
        public string[] Flags { get; set; }

        public int CanSell
        {
            get
            {
                if (Flags == null) return 1;
                if (Flags.Contains("NoSell") || Flags.Contains("AccountBound") || Flags.Contains("SoulbindOnAcquire"))
                {
                    return 0;
                }
                return 1;
            }
        }
    }
}
