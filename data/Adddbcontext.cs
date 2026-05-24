using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using test__api.data.models;

namespace test__api.data
{
    public class Adddbcontext(DbContextOptions<Adddbcontext> options) : DbContext(options)
    {
       public DbSet<Seller> Sallers { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
        
        
        
        
   
