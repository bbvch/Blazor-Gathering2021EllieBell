using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

namespace EllieGlöggli.Common.Admin
{
    public static class AdminExtension
    {
        public static void AddEllieAdmin(this IServiceCollection services)
        {
            services.AddLocalization();
            services.AddBlazoredLocalStorage();
            services.AddTransient<UserInfoService>();
        }
    }
}
