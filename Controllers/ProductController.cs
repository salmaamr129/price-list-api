using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PriceListApi.data.models;

namespace PriceListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = new List<Product>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Products", conn);
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        SellerID = reader.GetInt32(reader.GetOrdinal("SellerID"))
                    });
                }
            }

            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Product? product = null;

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Products WHERE ProductID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    product = new Product
                    {
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                        SellerID = reader.GetInt32(reader.GetOrdinal("SellerID"))
                    };
                }
            }

            if (product == null) return NotFound();
            return Ok(product);
        }

        
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "INSERT INTO Products (ProductID, ProductName, Price, Quantity, SellerID) " +
                    "VALUES (@id, @name, @price, @qty, @sellerId)", conn);

                cmd.Parameters.AddWithValue("@id", product.ProductID);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@qty", product.Quantity);
                cmd.Parameters.AddWithValue("@sellerId", product.SellerID);

                await cmd.ExecuteNonQueryAsync();
            }

            return Ok("Product added successfully!");
        }



        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Products SET ProductName=@name, Price=@price, Quantity=@qty, SellerID=@sellerId WHERE ProductID=@id", conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", product.ProductName);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@qty", product.Quantity);
                cmd.Parameters.AddWithValue("@sellerId", product.SellerID);

                int rows = await cmd.ExecuteNonQueryAsync();
                if (rows == 0) return NotFound("Product not found.");
            }

            return Ok("Product updated successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Products WHERE ProductID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = await cmd.ExecuteNonQueryAsync();
                if (rows == 0) return NotFound("Product not found.");
            }

            return Ok("Product deleted successfully!");
        }
    }
}





