using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Model.Types
{
    public class Recipe
    {
        public string RecipeId { get; set; }

        [JsonProperty("output_item_id")]
        public string OuputItemId { get; set; }

        [JsonProperty("output_item_count")]
        public int OutputItemCount { get; set; }

        [JsonProperty("ingredients")]
        public List<RecipeIngredient> Ingredients { get; set; }

        public override string ToString()
        {
            var res = OuputItemId + ", " + OutputItemCount + ", ";
            return Ingredients.Aggregate(res, (current, ingredient) => current + (ingredient.ItemId + ":"));
        }
    }

    public class RecipeIngredient
    {
        [JsonProperty("item_id")]
        public int ItemId { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
