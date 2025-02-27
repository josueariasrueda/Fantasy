using Fantasy.Backend.Repositories.Infraestructure.Interfaces;

namespace Fantasy.Backend.Repositories.Infraestructure.Implementation
{
    public class CurrentTenant : ICurrentTenant
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenant(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenant()
        {
            return _httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString() ?? "default-tenant";
        }
    }
}