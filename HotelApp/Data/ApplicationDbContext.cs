using HotelApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        // 6: Create DBContext(boiler plate code). Create options & connectionstring variables(boiler plate code).

        // DbSet-skikt för att representera tabellerna i databasen.
        // Varje DbSet skapar en "tabell" i databasen för respektive typ.
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<BookingRoom> BookingRooms { get; set; }


        /// <summary>
        /// Tom konstruktor: Denna tomma konstruktor behövs om du vill använda migrations
        /// (dvs. skapa databasen stegvis baserat på ändringar i datamodellen).
        /// </summary>
        public ApplicationDbContext()
        {
        }

        /// <summary>
        /// Konstruktor med alternativ (options):
        /// Denna konstruktor tar in inställningar som skickas från appens konfiguration,
        /// t.ex. anslutningssträngen.
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        /// <summary>
        /// Metoden `OnConfiguring`: används första gången applikationen körs för att
        /// koppla databasen till rätt server.
        /// Om anslutningssträngen inte redan är inställd, anger vi en direkt här.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.;Database=LakeVerdictResort;Trusted_Connection=True;TrustServerCertificate=true;");
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Konfigurera relationen mellan Booking och Invoice
        //    modelBuilder.Entity<Booking>()
        //        .HasOne(b => b.InvoiceInBooking) // Navigering från Booking till Invoice
        //        .WithOne(i => i.BookingInInvoice) // Navigering från Invoice till Booking
        //        .HasForeignKey<Invoice>(i => i.BookingId) // Invoice har främmande nyckeln
        //        .OnDelete(DeleteBehavior.Cascade); // Vid radering av Booking tas Invoice också bort

        //    base.OnModelCreating(modelBuilder); // Anropa basklassen om det behövs
        //}

    }
}
