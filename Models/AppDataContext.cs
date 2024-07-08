using Microsoft.EntityFrameworkCore;

namespace IMCCalculator.Models
{
    public class AppDataContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Imc> Imcs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=felipeandonini.db");
            }
        }
    }
}
