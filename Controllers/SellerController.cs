using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PriceListApi.data.models;

namespace PriceListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellerController : ControllerBase                                    // it's a class that inhert Controller base which help us in writing apis 
    {
        private readonly IConfiguration _configuration;                              // its a variable we use befor it iconfigrationcuse it make us able to reade data in any fille like joson

        public SellerController(IConfiguration configuration)                           // constructor it's parameters is IConfiguration configuration to gave the controller copy from this data 
        {
            _configuration= configuration;
        }
        private SqlConnection GetConnection() 
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
             
        
           
     


    
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seller>>> GetAllSellers()
        {
            var sellers = new List<Seller>();

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Sellers", conn);
                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    sellers.Add(new Seller
                    {
                        SellerID = reader.GetInt32(reader.GetOrdinal("SellerID")),
                        SellerName = reader.GetString(reader.GetOrdinal("SellerName")),
                        City = reader.GetString(reader.GetOrdinal("City"))
                    });
                }
            }

            return Ok(sellers);
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult<Seller>> GetSeller(int id)
        {
            Seller? seller = null;

            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT * FROM Sellers WHERE SellerID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    seller = new Seller
                    {
                        SellerID = reader.GetInt32(reader.GetOrdinal("SellerID")),
                        SellerName = reader.GetString(reader.GetOrdinal("SellerName")),
                        City = reader.GetString(reader.GetOrdinal("City"))
                    };
                }
            }

            if (seller == null) return NotFound();
            return Ok(seller);
        }

        
        [HttpPost]
        public async Task<ActionResult> CreateSeller(Seller seller)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "INSERT INTO Sellers (SellerID, SellerName, City) VALUES (@id, @name, @city)", conn);

                cmd.Parameters.AddWithValue("@id", seller.SellerID);
                cmd.Parameters.AddWithValue("@name", seller.SellerName);
                cmd.Parameters.AddWithValue("@city", seller.City);

                await cmd.ExecuteNonQueryAsync();
            }

            return Ok("Seller added successfully!");
        }

      
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSeller(int id, Seller seller)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(
                    "UPDATE Sellers SET SellerName=@name, City=@city WHERE SellerID=@id", conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", seller.SellerName);
                cmd.Parameters.AddWithValue("@city", seller.City);

                int rows = await cmd.ExecuteNonQueryAsync();
                if (rows == 0) return NotFound("Seller not found.");
            }

            return Ok("Seller updated successfully!");
        }

       
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSeller(int id)
        {
            using (var conn = GetConnection())
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM Sellers WHERE SellerID=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);

                int rows = await cmd.ExecuteNonQueryAsync();
                if (rows == 0) return NotFound("Seller not found.");
            }

            return Ok("Seller deleted successfully!");
        }
    }
}




