using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public static partial class ServicesExtension
    {
        public static IServiceCollection AddGeolocation(this IServiceCollection services)
        {
            services.AddTransient<Geolocation>();
            return services;
        }

        public static async Task<IServiceProvider> UseGeolocationAsync(this IServiceProvider services)
        {
            var js = services.GetRequiredService<IJSRuntime>();

            bool loaded;

            try
            {
                loaded = await js.InvokeAsync<bool>($"{Constant.JsAgent}.getLoadedFlag", nameof(Geolocation));
            }
            catch
            {
                loaded = false;
            }
            
            if (!loaded)
            {
                await js.InvokeVoidAsync($"{Constant.JsAgent}.loadJsCssFile", $"{Constant.ScriptPrefix}Geolocation.js", "js");
            }

            return services;
        }
    }
}
