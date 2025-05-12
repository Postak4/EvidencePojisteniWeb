using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EvidencePojisteniWeb.Models;

namespace EvidencePojisteniWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet pro jednotlivé entity
        public DbSet<PojistenecModel> Pojistenci { get; set; } = null!;
        public DbSet<PojisteniModel> Pojisteni { get; set; } = null!;
        public DbSet<PojisteniOsobyModel> PojisteneOsoby { get; set; } = null!;
        public DbSet<PojistnaUdalostModel> PojistneUdalosti { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1:N mezi PojistenecModel a PojisteniOsobyModel
            builder.Entity<PojisteniOsobyModel>()
                .HasOne(po => po.Osoba)
                .WithMany(o => o.PojisteniOsoby)              // Pokud PojistenecModel nemá kolekci PojisteniOsobyModel tak jen .WithMany()
                .HasForeignKey(po => po.OsobaId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1:N mezi PojisteniModel a PojisteniOsobyModel
            builder.Entity<PojisteniOsobyModel>()
                .HasOne(po => po.Pojisteni)
                .WithMany(p => p.PojisteniOsoby)              // Pokud PojisteniModel nemá kolekci PojisteniOsobyModel, tak jen .WithMany()
                .HasForeignKey(po => po.PojisteniId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enum typy (RoleVuciPojisteni, TypPojisteni) se ukládají jako číselné hodnoty
        }
    }
}
