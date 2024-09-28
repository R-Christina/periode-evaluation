using Microsoft.EntityFrameworkCore;
using periode.Models;

namespace periode.Data.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Evaluation> evaluation { get; set; }
        public DbSet<Etat_eval> etat_eval { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Evaluation>()
            .HasOne(e => e.etat)
            .WithMany(et => et.evaluation)
            .HasForeignKey(e => e.etat_id)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}