using System.Net;

namespace NexusOps.Web.Services
{
    public class ApiResultado<T>
    {
        public bool Exito { get; init; }
        public T? Datos { get; init; }
        public string? Error { get; init; }
        public HttpStatusCode Codigo { get; init; }
    }

    // La API respondió 401 con un token: la sesión terminó o el token expiró.
    public class SesionExpiradaException : Exception
    {
        public SesionExpiradaException() : base("La sesión ha expirado.") { }
    }
}
