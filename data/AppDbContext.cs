using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceListApi.data.models;

namespace PriceListApi.data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
       public DbSet<Seller> Sellers { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
        
        
        
        
   
