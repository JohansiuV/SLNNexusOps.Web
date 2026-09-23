using NexusOps.Web.Models;

namespace NexusOps.Web.Services
{
    public class DashboardService
    {
        private readonly ApiService _api;

        public DashboardService(ApiService api)
        {
            _api = api;
        }

        public Task<ApiResultado<DashboardResumenDto>> ObtenerResumenAsync() =>
            _api.GetAsync<DashboardResumenDto>("api/dashboard/resumen");
    }
}
