using System.ComponentModel.DataAnnotations;

namespace OfertareIndividuala.Models
{
    public class ClientNouFaraDateModel
    {
        [Required(ErrorMessage = "Numele și prenumele sunt obligatorii.")]
        public string Nume_prenume_client { get; set; }

        [Required(ErrorMessage = "Numărul de telefon este obligatoriu.")]
        [Phone(ErrorMessage = "Introduceți un număr de telefon valid.")]
        public string Nr_telefon_client { get; set; }

        [Required(ErrorMessage = "Emailul este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Introduceți un email valid.")]
        public string E_mail_client { get; set; }
    }
}