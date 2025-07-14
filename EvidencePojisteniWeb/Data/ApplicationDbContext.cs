using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EvidencePojisteniWeb.Models;

namespace EvidencePojisteniWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
                .WithMany(o => o.PojisteniOsoby)               // Pokud PojistenecModel nemá kolekci PojisteniOsobyModel tak jen .WithMany()
                .HasForeignKey(po => po.OsobaId)
                .OnDelete(DeleteBehavior.Cascade);             // při smazání pojištěnce se smažou i záznamy v PojisteneOsoby

            // 1:N mezi PojisteniModel a PojisteniOsobyModel
            builder.Entity<PojisteniOsobyModel>()
                .HasOne(po => po.Pojisteni)
                .WithMany(p => p.PojisteniOsoby)               // Pokud PojisteniModel nemá kolekci PojisteniOsobyModel, tak jen .WithMany()
                .HasForeignKey(po => po.PojisteniId)
                .OnDelete(DeleteBehavior.Restrict);            // zabránění kaskádovému smazání (NO ACTION) pro FK PojisteniId

            // Enum typy (RoleVuciPojisteni, TypPojisteni) se ukládají jako číselné hodnoty

            builder.Entity<PojisteniModel>()
                .Property(p => p.Castka)
                .HasPrecision(18, 2);                          // Nastavení přesnosti pro decimal (18 číslic celkem, 2 desetinná místa)

            builder.Entity<PojistnaUdalostModel>()
                .Property(u => u.Skoda)
                .HasPrecision(18, 2);                          // Nastavení přesnosti pro decimal (18 číslic celkem, 2 desetinná místa)

            // Mapování pro navigační vlastnosti
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Pojistenec)
                .WithOne(p => p.User)
                .HasForeignKey<ApplicationUser>(u => u.PojistenecModelId)
                .OnDelete(DeleteBehavior.SetNull);              // Při smazání uživatele se neodstraní pojištěnec, ale FK se nastaví na null
        }
    }
}

