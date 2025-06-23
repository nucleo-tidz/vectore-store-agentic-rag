using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using model;

namespace infrastructure.Repository
{
    internal class ProductRepository(IConfiguration configuration) : IProductRepository
    {
        public async Task<int> Save(ProductModel product)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("db"));
            string sql = @"INSERT INTO Product (Id,Name, Description)
                           VALUES (@Id,@Name, @Description)";
            return await connection.ExecuteAsync(sql, new { product.Id,product.Name, product.Description });
        }
    }
}
