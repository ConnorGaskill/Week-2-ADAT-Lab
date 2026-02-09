using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Week_2_ADAT_Lab.Models;

namespace Week_2_ADAT_Lab.Repositories
{
    public class CatalogRepositoryInefficient
    {
        private readonly string _connectionString;

        public CatalogRepositoryInefficient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Category> GetCatalog()
        {
            var categories = new List<Category>();

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            // Load categories
            using (SqlCommand cmd = new SqlCommand(
                "SELECT CategoryId, CategoryName, IsActive FROM dbo.Categories", conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryId = reader.GetInt32(0),
                        CategoryName = reader.GetString(1),
                        IsActive = reader.GetBoolean(2)
                    });
                }
            }

            foreach (var category in categories)
            {
                var subCategories = new List<SubCategory>();

                using (SqlCommand subCmd = new SqlCommand(
                    @"SELECT SubCategoryId, SubCategoryName
                      FROM dbo.SubCategories
                      WHERE CategoryId = @CategoryId", conn))
                {
                    subCmd.Parameters.Add("@CategoryId", System.Data.SqlDbType.Int)
                                     .Value = category.CategoryId;

                    using SqlDataReader subReader = subCmd.ExecuteReader();
                    {
                        while (subReader.Read())
                        {
                            subCategories.Add(new SubCategory
                            {
                                SubCategoryId = subReader.GetInt32(0),
                                CategoryId = category.CategoryId,
                                SubCategoryName = subReader.GetString(1)
                            });
                        }
                    }
                }

                category.SubCategories.AddRange(subCategories);

                foreach (var sub in subCategories)
                {
                    using SqlCommand prodCmd = new SqlCommand(
                        @"SELECT ProductId, ProductName, UnitPrice, IsActive
                          FROM dbo.Products
                          WHERE SubCategoryId = @SubCategoryId", conn);

                    prodCmd.Parameters.Add("@SubCategoryId", System.Data.SqlDbType.Int)
                                      .Value = sub.SubCategoryId;

                    using SqlDataReader prodReader = prodCmd.ExecuteReader();
                    {
                        while (prodReader.Read())
                        {
                            sub.Products.Add(new Product
                            {
                                ProductId = prodReader.GetInt32(0),
                                ProductName = prodReader.GetString(1),
                                UnitPrice = prodReader.GetDecimal(2),
                                IsActive = prodReader.GetBoolean(3)
                            });
                        }
                    }
                }
            }

            return categories;
        }
    }
}
