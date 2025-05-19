using System.ComponentModel.DataAnnotations;

namespace OfertareIndividuala.Models
{
    public class QuestionnaireModel
    {
        [Required(ErrorMessage = "Răspunsul este obligatoriu.")]
        public string SmartTvOffer { get; set; }

        [Required(ErrorMessage = "Răspunsul este obligatoriu.")]
        public string ElectronicsDiscount { get; set; }

        [Required(ErrorMessage = "Răspunsul este obligatoriu.")]
        public string WifiSatisfaction { get; set; }
    }
}