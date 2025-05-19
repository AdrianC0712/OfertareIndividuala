using Microsoft.EntityFrameworkCore;
using OfertareIndividuala.Models;

namespace OfertareIndividuala.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Utilizator_App { get; set; }
        public DbSet<SpecialOffer> Oferte_Speciale { get; set; }
        public DbSet<QuestionnaireResponse> QuestionnaireResponses { get; set; }
        public DbSet<ClientNouFaraDate> Client_nou_fara_date { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Utilizator_App");
            modelBuilder.Entity<User>().HasKey(u => u.Id_utilizator);

            modelBuilder.Entity<SpecialOffer>().ToTable("Oferte_Speciale");
            modelBuilder.Entity<SpecialOffer>().HasKey(o => o.Id_Oferta_Speciala);

            modelBuilder.Entity<ClientNouFaraDate>().ToTable("Client_nou_fara_date");
            modelBuilder.Entity<ClientNouFaraDate>().HasKey(c => c.Id_client_nou_fara_date);
        }
    }

    public class User
    {
        public int Id_utilizator { get; set; }
        public string Nume_utilizator { get; set; }
        public string Password_utilizator { get; set; }
        public string Type_of_utilizator { get; set; }
    }

    public class SpecialOffer
    {
        public int Id_Oferta_Speciala { get; set; }
        public string Denumire_Oferta_Speciala { get; set; }
        public int Type_Of_Oferta { get; set; }
        public string Suma_Oferta { get; set; }
        public string Suma_Magazin { get; set; }
    }

    public class QuestionnaireResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ClientNouId { get; set; }
        public string SmartTvOffer { get; set; }
        public string ElectronicsDiscount { get; set; }
        public string WifiSatisfaction { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public ClientNouFaraDate ClientNou { get; set; }
    }

    public class ClientNouFaraDate
    {
        public int Id_client_nou_fara_date { get; set; }
        public string Nume_prenume_client { get; set; }
        public string Nr_telefon_client { get; set; }
        public string E_mail_client { get; set; }
        public int? QuestionnaireResponsesId { get; set; }
        public int Utilizator_App_Id { get; set; }

        public QuestionnaireResponse QuestionnaireResponse { get; set; }
        //public User Utilizator { get; set; }
    }
}