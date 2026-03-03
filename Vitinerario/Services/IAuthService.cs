namespace Vitinerario.Services
{
    public interface IAuthService
    {
        Task<string> GetTokenAsync();
    }
}
