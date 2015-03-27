using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Model.Types;

namespace Model
{
    public class MySqlModel
    {
        private MySqlConnection m_sqlConnection;
        private MySqlCommand m_sqlCommand;
        private MySqlDataReader m_sqlDataReader;

        private string m_connectionString;

        public MySqlModel(string url, string username, string password)
        {

            m_connectionString = "Data Source=" + url + ";Initial Catalog=gw2recipe;User ID=" + username + ";Password=" + password;
            m_sqlConnection = new MySqlConnection(m_connectionString);
            Debug.WriteLine(m_sqlConnection.State);

        }

        public bool IsConnected()
        {
            try
            {
                m_sqlConnection.Open();
                Debug.WriteLine("Connection is open!");
                m_sqlConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            finally
            {
                m_sqlConnection.Close();
            }
            return false;
        }

        public int AddRecipe(Recipe recipe)
        {
            try
            {
                m_sqlConnection.Open();
                var sqlCommand =
                    new MySqlCommand(
                        "INSERT INTO recipe(recipeId,outputItemId, outputItemQuantity) VALUES (" + recipe.RecipeId + "," +
                        recipe.OuputItemId + "," + recipe.OutputItemCount + ");", m_sqlConnection);
                var insertedRows = sqlCommand.ExecuteNonQuery();

                foreach (var recipeIngredient in recipe.Ingredients)
                {
                    sqlCommand.CommandText =
                        "INSERT INTO recipe_ingredient(recipeId,ingredientId, ingredientQuantity) VALUES ("
                        + recipe.RecipeId + "," + recipeIngredient.ItemId + "," + recipeIngredient.Count + ");";
                    insertedRows += sqlCommand.ExecuteNonQuery();
                }

                m_sqlConnection.Close();
                return insertedRows;
            }
            catch (Exception)
            {
                Debug.WriteLine("sql insert failed");
                m_sqlConnection.Close();
                return -1;
            }
        }

        public void AddRecipeList(List<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                AddRecipe(recipe);
            }
        }

        public void AddItemList(List<Item> items)
        {
            m_sqlConnection.Open();
            foreach (var item in items)
            {
                AddItem(item);
            }
            m_sqlConnection.Close();
        } 

        public int AddItem(Item item)
        {
            try
            {
                var sqlCommand =
                    new MySqlCommand(
                        "INSERT IGNORE INTO item(itemId , itemName , canSell) VALUES (" + item.Id + ",'" +
                        item.Name.Replace('\'', '´') + "'," + item.CanSell + ");", m_sqlConnection);
                var insertedRows = sqlCommand.ExecuteNonQuery();

                return insertedRows;
            }
            catch (MySqlException)
            {
                
                var newConnection = new MySqlConnection(m_connectionString);
                try
                {
                    newConnection.Open();
                    var sqlCommand =
                        new MySqlCommand(
                            "INSERT IGNORE INTO item(itemId , itemName , canSell) VALUES (" + item.Id + ",'" +
                            item.Name.Replace('\'', '´') + "'," + item.CanSell + ");", m_sqlConnection);
                    var insertedRows = sqlCommand.ExecuteNonQuery();
                    newConnection.Close();
                    return insertedRows;
                }

                catch (Exception)
                {
                    return - 1;
                }
                
            }
            catch (Exception e)
            {
                Debug.WriteLine("sql insert failed");
                Debug.WriteLine(e.Message);
                return -1;
            }
        }

        public List<string> OutputItemIdList()
        {
            var outputItemIdList = new List<string>();
            try
            {
                m_sqlConnection.Open();
                m_sqlCommand =
                    new MySqlCommand(
                        "SELECT outputItemId FROM recipe group by outputItemId;", m_sqlConnection);
                m_sqlDataReader = m_sqlCommand.ExecuteReader();
                while (m_sqlDataReader.Read())
                {
                    outputItemIdList.Add(m_sqlDataReader.GetInt32(0)+"");
                    Debug.WriteLine(outputItemIdList.Last());
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                m_sqlConnection.Close();
            }
            return outputItemIdList;
        }

        public void Open()
        {
            m_sqlConnection.Open();
        }
        public void Close()
        {
            m_sqlConnection.Close();
        }

        public List<string> IngredientItemIdList()
        {
            var outputItemIdList = new List<string>();
            try
            {
                m_sqlConnection.Open();
                m_sqlCommand =
                    new MySqlCommand(
                        "SELECT * FROM gw2recipe.recipe_ingredient where ingredientId not in (select item.itemId from item) group by ingredientId", m_sqlConnection);
                m_sqlDataReader = m_sqlCommand.ExecuteReader();
                while (m_sqlDataReader.Read())
                {
                    outputItemIdList.Add(m_sqlDataReader.GetInt32(0) + "");
                    Debug.WriteLine(outputItemIdList.Last());
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }
            finally
            {
                m_sqlConnection.Close();
            }
            return outputItemIdList;
        }
    }
}
