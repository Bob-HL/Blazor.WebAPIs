using Microsoft.Extensions.DependencyInjection;

namespace Cutec.Blazor.WebAPIs
{
    public static partial class ServicesExtension
    {
        public static IServiceCollection AddWebStorage(this IServiceCollection services)
        {
            services.AddSingleton<LocalStorage>();
            services.AddSingleton<SessionStorage>();
            return services;
        }
    }
}
