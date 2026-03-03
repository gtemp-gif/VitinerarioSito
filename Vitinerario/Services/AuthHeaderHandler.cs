using System.Net.Http.Headers;

namespace Vitinerario.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        public AuthHeaderHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authService.GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
