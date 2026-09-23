using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NexusOps.Web.Models;

namespace NexusOps.Web.Services
{
    public class AuthService
    {
        public const string ClaimNombreCompleto = "NombreCompleto";

        private readonly ApiService _api;

        public AuthService(ApiService api)
        {
            _api = api;
        }

        public Task<ApiResultado<LoginResponseDto>> LoginAsync(LoginRequestDto dto) =>
            _api.PostAsync<LoginResponseDto>("api/auth/login", dto);

        // Crea la cookie de sesión con el rol del usuario y el token JWT de la API.
        public async Task IniciarSesionAsync(HttpContext contexto, LoginResponseDto datos)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, datos.IdUsuario.ToString()),
                new(ClaimTypes.Name, datos.NombreUsuario),
                new(ClaimTypes.Role, datos.Rol),
                new(ClaimNombreCompleto, datos.NombreCompleto)
            };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var propiedades = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = datos.Expira.ToUniversalTime()
            };
            propiedades.StoreTokens(new[]
            {
                new AuthenticationToken { Name = TokenHandler.NombreToken, Value = datos.Token }
            });

            await contexto.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad), propiedades);
        }

        public Task CerrarSesionAsync(HttpContext contexto) =>
            contexto.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
