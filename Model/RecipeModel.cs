using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.XPath;
using Model.Types;
using Newtonsoft.Json;

namespace Model
{
    public class RecipeModel
    {
        private WebClient webClient;

        public RecipeModel()
        {
            webClient = new WebClient {Proxy = null};
        }

        public List<string> GetRecipeIdList()
        {
            var json  = webClient.DownloadString("https://api.guildwars2.com/v2/recipes/");

            var jsonConverted = JsonConvert.DeserializeObject<List<string>>(json);

            return jsonConverted;
        }

        public Recipe GetRecipeFromId(string id)
        {
            var json  = webClient.DownloadString("https://api.guildwars2.com/v2/recipes/" + id);

            var recipe = JsonConvert.DeserializeObject<Recipe>(json);
            recipe.RecipeId = id;
            return recipe;
        }

        public Commerce GetCommerceFromItemId(string id)
        {
            try
            {
                var json  = webClient.DownloadString("https://api.guildwars2.com/v2/commerce/prices/" + id);

                var commerce = JsonConvert.DeserializeObject<Commerce>(json);

                return commerce;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<double> ProfitFromInstasellRecipeWhenBuyIngredients(Recipe recipe)
        {
            var profit = 0.0;
            try
            {
                var outputCommerce = Task.Run(() => GetCommerceFromItemId(recipe.OuputItemId + ""));
                var ingredientsCommerce = recipe.Ingredients.Select(ingredient => Task.Run(() => GetCommerceFromItemId(ingredient.ItemId + ""))).ToList();

                await outputCommerce;
                foreach (var task in ingredientsCommerce)
                {
                    await task;
                }

                profit += outputCommerce.Result.GetBuy.UnitPrice * recipe.OutputItemCount * 0.85;
                for (int i = 0; i < ingredientsCommerce.Count; i++)
                {
                    profit -= ingredientsCommerce[i].Result.GetBuy.UnitPrice * recipe.Ingredients[i].Count;
                }

                return profit;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public Item[] ItemListFromIdsString(string ids)
        {
            var json = new WebClient() { Proxy = null }.DownloadString("https://api.guildwars2.com/v2/items?ids=" + ids);
            var item = JsonConvert.DeserializeObject<Item[]>(json);

            return item;
        }

        public Item ItemFromItemId(string id)
        {
            var json = new WebClient(){Proxy = null}.DownloadString("https://api.guildwars2.com/v2/items/" + id);
            var item = JsonConvert.DeserializeObject<Item>(json);

            return item;
        }

        public async Task<Dictionary<string, double>> GetMostProffitableRecipe()
        {
            var recipeIds = GetRecipeIdList();

            var recipeIdWithProfit = new Dictionary<string, double>();
            Debug.WriteLine("Before For Loop");
            foreach (var id in recipeIds)
            {
                var recipe = GetRecipeFromId(id);
                var profit = await ProfitFromInstasellRecipeWhenBuyIngredients(recipe);
                if (!(profit > 0)) continue;
                recipeIdWithProfit.Add(id, profit);
                Debug.WriteLine("Item: " + ItemFromItemId(recipe.OuputItemId).Name + ", Profit: " + profit);
            }
            Debug.WriteLine("After For Loop");

            var sortedRecipes = recipeIdWithProfit.OrderByDescending(pair => pair.Value);

            foreach (var sortedRecipe in sortedRecipes)
            {
                Debug.WriteLine(ItemFromItemId(sortedRecipe.Key).Name + ", " + sortedRecipe.Value);
            }

            return sortedRecipes.ToDictionary(pair => pair.Key, pair => pair.Value);

        }
    }
}
