using System.Threading.Tasks;
using Vitinerario.Models;

namespace Vitinerario.Services
{
    public interface IEmailService
    {
        Task<bool> SendContactEmailAsync(ContactFormViewModel model);
    }
}
