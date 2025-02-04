//using IdentityRefreshToken.Models;
//using Microsoft.EntityFrameworkCore;

//namespace IdentityRefreshToken.Data
//{
//    public class HouseDbContext : DbContext
//    {
//        public DbSet<House> Houses { get; set; }
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlServer(@"Server=DESKTOP-CBCGBLG;
//                                          Database=HouseDB;
//                                          Integrated security=True;
//                                          TrustServerCertificate=True");
//            base.OnConfiguring(optionsBuilder);
//        }

//        public HouseDbContext()
//        {

//        }
//    }   
//}

using IdentityRefreshToken.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityRefreshToken.Data
{
    public class HouseDbContext : DbContext
    {
        public HouseDbContext(DbContextOptions<HouseDbContext> options)
            : base(options)
        {
        }

        public DbSet<House> Houses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-CBCGBLG;Database=HouseDB;Integrated Security=True;TrustServerCertificate=True");
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}

