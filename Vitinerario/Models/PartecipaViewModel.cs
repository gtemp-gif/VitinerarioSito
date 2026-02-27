using System.ComponentModel.DataAnnotations;

namespace Vitinerario.Models
{
    public class PartecipaViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        public string Cognome { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria.")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleziona un evento.")]
        public string Evento { get; set; } = string.Empty;
    }
}
