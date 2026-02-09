using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Week_2_ADAT_Lab.Models;

namespace Week_2_ADAT_Lab.Repositories
{
    public class CatalogRepositoryEfficient
    {
        private readonly string _connectionString;

        public CatalogRepositoryEfficient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Category> GetCatalog()
        {
            var categories = new Dictionary<int, Category>();
            var subCategories = new Dictionary<int, SubCategory>();

            string sql = @"
                SELECT
                    c.CategoryId,
                    c.CategoryName,
                    c.IsActive AS CategoryIsActive,

                    sc.SubCategoryId,
                    sc.SubCategoryName,

                    p.ProductId,
                    p.ProductName,
                    p.UnitPrice,
                    p.IsActive AS ProductIsActive
                FROM dbo.Categories c
                LEFT JOIN dbo.SubCategories sc ON c.CategoryId = sc.CategoryId
                LEFT JOIN dbo.Products p ON sc.SubCategoryId = p.SubCategoryId
                ORDER BY c.CategoryId, sc.SubCategoryId, p.ProductId;
            ";

            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            {
                while (reader.Read())
                {
                    int categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));

                    if (!categories.TryGetValue(categoryId, out Category category))
                    {
                        category = new Category
                        {
                            CategoryId = categoryId,
                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("CategoryIsActive"))
                        };

                        categories.Add(categoryId, category);
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("SubCategoryId")))
                    {
                        int subCategoryId = reader.GetInt32(reader.GetOrdinal("SubCategoryId"));

                        if (!subCategories.TryGetValue(subCategoryId, out SubCategory sub))
                        {
                            sub = new SubCategory
                            {
                                SubCategoryId = subCategoryId,
                                CategoryId = categoryId,
                                SubCategoryName = reader.GetString(reader.GetOrdinal("SubCategoryName"))
                            };

                            subCategories.Add(subCategoryId, sub);
                            category.SubCategories.Add(sub);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                        {
                            sub.Products.Add(new Product
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("ProductIsActive"))
                            });
                        }
                    }
                }
            }

            return new List<Category>(categories.Values);
        }
    }
}
