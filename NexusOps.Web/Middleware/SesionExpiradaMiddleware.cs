using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NexusOps.Web.Services;

namespace NexusOps.Web.Middleware
{
    public class SesionExpiradaMiddleware
    {
        private readonly RequestDelegate _siguiente;

        public SesionExpiradaMiddleware(RequestDelegate siguiente)
        {
            _siguiente = siguiente;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            try
            {
                await _siguiente(contexto);
            }
            catch (SesionExpiradaException) when (!contexto.Response.HasStarted)
            {
                await contexto.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                contexto.Response.Redirect("/Login?expirada=true");
            }
        }
    }
}
