using System.Globalization;
using System.Text;

namespace NexusOps.Web.Helpers
{
    public static class CsvHelper
    {
        // CSV con BOM UTF-8 para que Excel muestre correctamente las tildes.
        public static byte[] Generar(IEnumerable<string> cabeceras, IEnumerable<IEnumerable<object?>> filas)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", cabeceras.Select(Escapar)));

            foreach (var fila in filas)
                sb.AppendLine(string.Join(",", fila.Select(v => Escapar(Formatear(v)))));

            var preambulo = new UTF8Encoding(true).GetPreamble();
            var contenido = Encoding.UTF8.GetBytes(sb.ToString());
            return preambulo.Concat(contenido).ToArray();
        }

        private static string Formatear(object? valor) => valor switch
        {
            null => string.Empty,
            DateTime d => d.TimeOfDay == TimeSpan.Zero ? d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                                                       : d.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
            decimal m => m.ToString("0.00", CultureInfo.InvariantCulture),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => valor.ToString() ?? string.Empty
        };

        private static string Escapar(string texto)
        {
            // Evita la inyección de fórmulas al abrir el CSV en Excel.
            if (texto.Length > 0 && "=+-@".Contains(texto[0]) && !decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                texto = "'" + texto;

            return texto.Contains('"') || texto.Contains(',') || texto.Contains('\n') || texto.Contains('\r')
                ? "\"" + texto.Replace("\"", "\"\"") + "\""
                : texto;
        }
    }
}
