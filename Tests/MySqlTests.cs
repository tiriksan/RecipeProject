using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Types;

namespace Tests
{
    [TestClass]
    public class MySqlTests
    {
        private MySqlModel m_sqlModel;

        [TestInitialize]
        public void InstantiateModel()
        {
            m_sqlModel = new MySqlModel("localhost", "root", "15Hurp16Durp");
        }

        [TestMethod]
        public void IsConnected_True()
        {
            var isConnected = m_sqlModel.IsConnected();

            Assert.IsTrue(isConnected);
        }

        [TestMethod]
        public void AddRecipe_Id1()
        {
            var recipeModel = new RecipeModel();

            var recipe = recipeModel.GetRecipeFromId("1");

            var sqlSuccess = m_sqlModel.AddRecipe(recipe);

            Assert.AreEqual(recipe.Ingredients.Count + 1, sqlSuccess);
        }

        [TestMethod]
        public void AddRecipeList_AddAllRecipes()
        {
            var recipeModel = new RecipeModel();

            Debug.WriteLine("PreIdList");

            var recipeIds = recipeModel.GetRecipeIdList();

            Debug.WriteLine("PreRecipes");

            var recipeList = new List<Recipe>();

            foreach (var recipeId in recipeIds)
            {
                Debug.WriteLine(recipeId + ":" + recipeIds.IndexOf(recipeId) + "/" + recipeIds.Count);
                recipeList.Add(recipeModel.GetRecipeFromId(recipeId));
            }

            //var recipeList = recipeIds.Select(recipeId => recipeModel.GetRecipeFromId(recipeId)).ToList();

            Debug.WriteLine("PostRecipes");

            m_sqlModel.AddRecipeList(recipeList);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void AddItem_id3948()
        {
            var recipeModel = new RecipeModel();
            var item3948 = recipeModel.ItemFromItemId("3948");

            Assert.IsTrue(item3948.Id == "3948");
            Debug.WriteLine(item3948.Name);
            Assert.AreEqual(0, item3948.CanSell);

            var sqlInsert = m_sqlModel.AddItem(item3948);

            Assert.AreEqual(1, sqlInsert);

        }

        [TestMethod]
        public void OutputItemList_GetAllOutputItemIds()
        {
            var itemIdList = m_sqlModel.OutputItemIdList();
            var numberOfIds = itemIdList.Count;
            Debug.WriteLine(numberOfIds);

            Assert.AreEqual(8945, numberOfIds);
        }

        [TestMethod]
        public void AddAllOutputItems()
        {
            var recipeModel = new RecipeModel();
            var itemIdList = m_sqlModel.OutputItemIdList();

            var itemList = new List<Item>();

            Debug.WriteLine("Start ItemList");

            Parallel.ForEach(itemIdList, itemId =>
            {
                itemList.Add(recipeModel.ItemFromItemId(itemId));
            });

            Debug.WriteLine("Start Add to db");

            try
            {
                m_sqlModel.Open();

                Parallel.ForEach(itemList, item =>
                {
                    var sqlInsert = m_sqlModel.AddItem(item);
                    if (sqlInsert < 0)
                    {
                        Debug.WriteLine("Something wrong");
                    }
                });
            }
            catch (Exception)
            {
                m_sqlModel.Close();
            }
        }
        [TestMethod]
        public void OutputItemIdsFromIdsStringAndAddToDb()
        {
            var recipeModel = new RecipeModel();
            var itemIdList = m_sqlModel.OutputItemIdList();

            Debug.WriteLine(itemIdList.Count() + "");
            const int numItemsAtTheSameTime = 200;
            var idsString = "";
            var items = new List<Item>();
            int index = 0;
            while (index < itemIdList.Count())
            {
                if ((index)%numItemsAtTheSameTime == 0)
                {
                    idsString += itemIdList[index];
                }
                else
                {
                    idsString += "," + itemIdList[index];
                }
                if ((index+1) % numItemsAtTheSameTime == 0 || index == itemIdList.Count-1)
                {
                    //Debug.WriteLine("IdsString: " + idsString);
                    var itemArray = recipeModel.ItemListFromIdsString(idsString);
                    //Debug.WriteLine("ItemsCount: " + itemArray.Count());
                    items.AddRange(itemArray);
                    idsString = "";
                }
                index ++;
            }

            Debug.WriteLine("Inserting into DB:");

            m_sqlModel.AddItemList(items);

            Debug.WriteLine("Done");
            

        }

        [TestMethod]
        public void IngredientItemIdsFromIdsStringAndAddToDb()
        {
            var recipeModel = new RecipeModel();
            var itemIdList = m_sqlModel.IngredientItemIdList();

            Debug.WriteLine(itemIdList.Count() + "");
            const int numItemsAtTheSameTime = 200;
            var idsString = "";
            var items = new List<Item>();
            int index = 0;
            while (index < itemIdList.Count())
            {
                if ((index) % numItemsAtTheSameTime == 0)
                {
                    idsString += itemIdList[index];
                }
                else
                {
                    idsString += "," + itemIdList[index];
                }
                if ((index + 1) % numItemsAtTheSameTime == 0 || index == itemIdList.Count - 1)
                {
                    //Debug.WriteLine("IdsString: " + idsString);
                    var itemArray = recipeModel.ItemListFromIdsString(idsString);
                    //Debug.WriteLine("ItemsCount: " + itemArray.Count());
                    items.AddRange(itemArray);
                    idsString = "";
                }
                index++;
            }

            Debug.WriteLine("Inserting into DB:");

            m_sqlModel.AddItemList(items);

            Debug.WriteLine("Done");


        }
    }
}
