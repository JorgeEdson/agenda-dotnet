using System.Security.Claims;

namespace Agenda.API.Utils
{
    public interface IHttpContextAcessorService
    {
        long GetId();
        GetInfoPerfil GetPermissoes();
        bool IsAuthenticated();

    }

    public sealed class HttpContextAcessorUtil(IHttpContextAccessor httpContextAccessor) : IHttpContextAcessorService
    {
        public IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;

        public long GetId()
        {
            var id = HttpContextAccessor?
                .HttpContext?.User?.FindFirstValue(ClaimTypes.PrimarySid);

            return id != null
                ? long.Parse(id)
                : 0;
        }

        public GetInfoPerfil GetPermissoes()
        {
            if (HttpContextAccessor.HttpContext?.User?.Claims != null)
            {
                string id = HttpContextAccessor.HttpContext?.User?.Claims
                    .Where(x => x.Type == ClaimTypes.PrimarySid)
                    .Select(x => x.Value)
                    .FirstOrDefault();

                string email = HttpContextAccessor.HttpContext?.User?.Claims
                    .Where(x => x.Type == ClaimTypes.Email)
                    .Select(x => x.Value)
                    .FirstOrDefault();

                var roles = HttpContextAccessor.HttpContext.User.Claims
                    .Where(x => x.Type == ClaimTypes.Role)
                    .Select(x => x.Value)
                    .ToArray();

                var info = new GetInfoPerfil
                {
                    Id = id,
                    Email = email,
                    Roles = roles
                };

                return info;
            }

            return new GetInfoPerfil
            {
                Id = "0",
                Email = string.Empty,
                Roles = new string[] { }
            };
        }

        public bool IsAuthenticated()
        {
            return HttpContextAccessor?
                .HttpContext.User.Identity?.IsAuthenticated ?? false;
        }
    }

    public class GetInfoPerfil
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
