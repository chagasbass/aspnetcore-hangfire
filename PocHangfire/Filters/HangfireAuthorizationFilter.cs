using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Aspnetcore.Hangfire.Filters
{
    /// <summary>
    /// Filtro para autenticação e autorização da Dashboard
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {

            /*TODO:
             *Verificar qual seria a lógica e se é necessário validar o usuario para acesso da dashboard. 
             * no momento todos os users acessam.
             */

            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}
