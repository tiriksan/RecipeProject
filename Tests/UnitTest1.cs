using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        private RecipeModel m_recipeModel;

        [TestInitialize]
        public void Initialize()
        {
            m_recipeModel = new RecipeModel();

        }

        [TestMethod]
        public void GetRecipeIdList_GetAll()
        {

            var list = m_recipeModel.GetRecipeIdList();

            foreach (var jToken in list)
            {
                Debug.WriteLine(jToken);
            }

            Assert.IsTrue(list.GetType() == typeof(List<string>));
        }

        [TestMethod]
        public void GetRecipeFromId_GetNumber1()
        {
            var recipe = m_recipeModel.GetRecipeFromId("1");

            Debug.WriteLine(recipe.ToString());
        }

        [TestMethod]
        public void GetCommerceFromItemId_GetOmnomberryBar()
        {
            var omnomberrybarCommerce = m_recipeModel.GetCommerceFromItemId("12452");

            Debug.WriteLine("Buy unit price: " + omnomberrybarCommerce.GetBuy.UnitPrice);

            Debug.WriteLine("Sell unit price: " + omnomberrybarCommerce.GetSell.UnitPrice);

        }

        [TestMethod]
        public void GetProfitFromRecipe()
        {
            var start = DateTime.Now;
            var profitFromRecipe = m_recipeModel.ProfitFromInstasellRecipeWhenBuyIngredients(m_recipeModel.GetRecipeFromId("20"));
            var end = DateTime.Now;

            Debug.WriteLine(end.Subtract(start));

            Debug.WriteLine(profitFromRecipe);

        }

        [TestMethod]
        public void GetMostProffitableRecipe_InstaSellAndBuyIngredients()
        {
            var recipes = m_recipeModel.GetMostProffitableRecipe();
            recipes.Wait();
            foreach (var recipe in recipes.Result)
            {
                Debug.WriteLine(recipe.Value);
            }
            
        }

        [TestMethod]
        public void GetItemArrayFromIdsString()
        {
            var itemArray = m_recipeModel.ItemListFromIdsString("1,2,3,4");
            foreach (var item in itemArray)
            {
                Debug.WriteLine(item.Name);
            }
        }

    }
}
