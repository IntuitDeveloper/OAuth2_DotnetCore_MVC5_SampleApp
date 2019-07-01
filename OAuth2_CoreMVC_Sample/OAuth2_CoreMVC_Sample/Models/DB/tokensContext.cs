using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OAuth2_CoreMVC_Sample.Models.DB
{
    public partial class tokensContext : DbContext
    {
        public tokensContext()
        {
        }

        public tokensContext(DbContextOptions<tokensContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Token> Token { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
              //  optionsBuilder.UseSqlServer("Server=localhost;Database=tokens;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.RealmId);

                entity.Property(e => e.RealmId)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.AccessToken)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(1000);
            });
        }
    }
}
