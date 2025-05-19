using System.ComponentModel.DataAnnotations;

namespace OfertareIndividuala.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Numele de utilizator este obligatoriu.")]
        [EmailAddress(ErrorMessage = "Introduceți un email valid.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@starnet\.md$", ErrorMessage = "Emailul trebuie să fie de forma nume@starnet.md.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Parola este obligatorie.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Parola trebuie să aibă minim 8 caractere.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}