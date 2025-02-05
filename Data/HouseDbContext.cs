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
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; } // Add User table

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=DESKTOP-CBCGBLG;Database=HouseDB;Integrated Security=True;TrustServerCertificate=True");
            }

            base.OnConfiguring(optionsBuilder);

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // Call base method

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(r => r.Id); // Primary Key
                entity.Property(r => r.Token)
                    .IsRequired()
                    .HasMaxLength(500); // Optional max length
                entity.Property(r => r.ExpiryDate)
                    .IsRequired();
            });
        }
    }
}

