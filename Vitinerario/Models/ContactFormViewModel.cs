using System.ComponentModel.DataAnnotations;

namespace Vitinerario.Models
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Il campo Nome Completo è obbligatorio.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il campo Email è obbligatorio.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il campo Messaggio è obbligatorio.")]
        public string Message { get; set; } = string.Empty;
    }
}
