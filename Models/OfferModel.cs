using System.ComponentModel.DataAnnotations;

namespace OfertareIndividuala.Models
{
    public class OfferModel
    {
        public int Id_Oferta_Speciala { get; set; }

        [Required(ErrorMessage = "Denumirea ofertei este obligatorie.")]
        [StringLength(100, ErrorMessage = "Denumirea nu poate avea mai mult de 100 de caractere.")]
        public string Denumire_Oferta_Speciala { get; set; }

        [Required(ErrorMessage = "Tipul ofertei este obligatoriu.")]
        [Range(1, int.MaxValue, ErrorMessage = "Tipul ofertei trebuie să fie un număr pozitiv.")]
        public int Type_Of_Oferta { get; set; }

        [Required(ErrorMessage = "Suma ofertei este obligatorie.")]
        [StringLength(50, ErrorMessage = "Suma ofertei nu poate avea mai mult de 50 de caractere.")]
        public string Suma_Oferta { get; set; }

        [Required(ErrorMessage = "Suma magazin este obligatorie.")]
        [StringLength(50, ErrorMessage = "Suma magazin nu poate avea mai mult de 50 de caractere.")]
        public string Suma_Magazin { get; set; }
    }
}