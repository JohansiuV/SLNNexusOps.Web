namespace NexusOps.Web.Models
{
    public class PaginadoDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamano { get; set; } = 10;
        public int TotalPaginas => Tamano > 0 ? (int)Math.Ceiling(Total / (double)Tamano) : 0;
    }
}
