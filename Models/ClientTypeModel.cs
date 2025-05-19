using System.ComponentModel.DataAnnotations;

namespace OfertareIndividuala.Models
{
    public class ClientTypeModel
    {
        [Required(ErrorMessage = "Tipul clientului este obligatoriu.")]
        public string ClientType { get; set; }
    }
}