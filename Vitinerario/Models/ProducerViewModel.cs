using System.ComponentModel.DataAnnotations;

namespace Vitinerario.Models
{
    public class ProducerViewModel
    {
        [Required(ErrorMessage = "Il nome dell'azienda è obbligatorio.")]
        [Display(Name = "Nome Azienda")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il referente è obbligatorio.")]
        [Display(Name = "Referente")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'indirizzo email è obbligatorio.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il messaggio è obbligatorio.")]
        [Display(Name = "Messaggio")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleziona un tipo di richiesta.")]
        [Display(Name = "Tipo di Richiesta")]
        public string RequestType { get; set; } = string.Empty;
    }
}
