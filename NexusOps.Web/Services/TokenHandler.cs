using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace NexusOps.Web.Services
{
    // Agrega el token JWT del usuario autenticado a cada petición hacia la API.
    public class TokenHandler : DelegatingHandler
    {
        public const string NombreToken = "access_token";

        private readonly IHttpContextAccessor _accesor;

        public TokenHandler(IHttpContextAccessor accesor)
        {
            _accesor = accesor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var contexto = _accesor.HttpContext;
            if (contexto != null)
            {
                var token = await contexto.GetTokenAsync(NombreToken);
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
