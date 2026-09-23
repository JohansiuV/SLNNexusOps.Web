using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace NexusOps.Web.Services
{
    // Único punto de comunicación entre la aplicación Web y la API REST.
    // La Web NUNCA se conecta a SQL Server: todo pasa por aquí.
    public class ApiService
    {
        private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        private readonly IHttpClientFactory _factory;

        public ApiService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public Task<ApiResultado<T>> GetAsync<T>(string url) => EnviarAsync<T>(HttpMethod.Get, url, null);

        public Task<ApiResultado<T>> PostAsync<T>(string url, object? cuerpo = null) => EnviarAsync<T>(HttpMethod.Post, url, cuerpo);

        public Task<ApiResultado<T>> PutAsync<T>(string url, object? cuerpo = null) => EnviarAsync<T>(HttpMethod.Put, url, cuerpo);

        public Task<ApiResultado<object>> DeleteAsync(string url) => EnviarAsync<object>(HttpMethod.Delete, url, null);

        // Arma "ruta?clave=valor&..." omitiendo valores vacíos.
        public static string Url(string ruta, params (string Clave, object? Valor)[] parametros)
        {
            var sb = new StringBuilder();
            foreach (var (clave, valor) in parametros)
            {
                var texto = valor switch
                {
                    null => null,
                    string s => string.IsNullOrWhiteSpace(s) ? null : s.Trim(),
                    bool b => b ? "true" : "false",
                    DateTime d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                    _ => valor.ToString()
                };

                if (string.IsNullOrEmpty(texto)) continue;

                sb.Append(sb.Length == 0 ? '?' : '&');
                sb.Append(Uri.EscapeDataString(clave)).Append('=').Append(Uri.EscapeDataString(texto));
            }

            return ruta + sb;
        }

        private async Task<ApiResultado<T>> EnviarAsync<T>(HttpMethod metodo, string url, object? cuerpo)
        {
            var cliente = _factory.CreateClient("Api");
            using var peticion = new HttpRequestMessage(metodo, url);

            if (cuerpo != null)
            {
                peticion.Content = JsonContent.Create(cuerpo, cuerpo.GetType(), null, Json);
            }

            try
            {
                using var respuesta = await cliente.SendAsync(peticion);
                var texto = await respuesta.Content.ReadAsStringAsync();

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized && !url.Contains("auth/login", StringComparison.OrdinalIgnoreCase))
                {
                    throw new SesionExpiradaException();
                }

                if (respuesta.IsSuccessStatusCode)
                {
                    T? datos = default;
                    if (!string.IsNullOrWhiteSpace(texto) && typeof(T) != typeof(object))
                    {
                        datos = JsonSerializer.Deserialize<T>(texto, Json);
                    }

                    return new ApiResultado<T> { Exito = true, Datos = datos, Codigo = respuesta.StatusCode };
                }

                return new ApiResultado<T>
                {
                    Exito = false,
                    Codigo = respuesta.StatusCode,
                    Error = ExtraerMensaje(texto, respuesta.StatusCode)
                };
            }
            catch (HttpRequestException)
            {
                return Fallo<T>("No se pudo conectar con la API. Verifique que NexusOps.Api esté en ejecución.");
            }
            catch (TaskCanceledException)
            {
                return Fallo<T>("La API tardó demasiado en responder. Intente nuevamente.");
            }
            catch (JsonException)
            {
                return Fallo<T>("La API devolvió una respuesta que no se pudo interpretar.");
            }
        }

        private static ApiResultado<T> Fallo<T>(string mensaje) =>
            new() { Exito = false, Error = mensaje, Codigo = HttpStatusCode.ServiceUnavailable };

        // La API responde errores como { "mensaje": "..." }.
        private static string ExtraerMensaje(string texto, HttpStatusCode codigo)
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                try
                {
                    using var doc = JsonDocument.Parse(texto);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                        doc.RootElement.TryGetProperty("mensaje", out var mensaje) &&
                        mensaje.ValueKind == JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(mensaje.GetString()))
                    {
                        return mensaje.GetString()!;
                    }
                }
                catch (JsonException)
                {
                    // cuerpo no JSON: se usa el mensaje genérico
                }
            }

            return codigo switch
            {
                HttpStatusCode.Forbidden => "No tiene permisos para realizar esta operación.",
                HttpStatusCode.NotFound => "El registro solicitado no existe.",
                HttpStatusCode.Conflict => "La operación entra en conflicto con el estado actual del registro.",
                HttpStatusCode.BadRequest => "Los datos enviados no son válidos.",
                _ => "Ocurrió un error al procesar la solicitud."
            };
        }
    }
}
