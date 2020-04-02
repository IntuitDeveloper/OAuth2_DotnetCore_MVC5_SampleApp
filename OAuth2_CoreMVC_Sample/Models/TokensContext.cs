using Microsoft.EntityFrameworkCore;

namespace OAuth2_CoreMVC_Sample.Models
{
    public class TokensContext : DbContext
    {
        public TokensContext()
        {
        }

        public TokensContext(DbContextOptions<TokensContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Token> Token { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
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