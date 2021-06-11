using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public static partial class ServicesExtension
    {
        public static IServiceCollection AddBlazorWebAPIs(this IServiceCollection services)
        {
            services.AddGeolocation()
                .AddIndexedDB<IndexedDb>()
                .AddWebStorage();
            return services;
        }

        /// <summary>
        /// It is no need if All.js already included in index.html.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static async Task<IServiceProvider> UseBlazorWebAPIsAsync(this IServiceProvider services)
        {
            var js = services.GetRequiredService<IJSRuntime>();
            await js.InvokeVoidAsync($"{Constant.JsAgent}loadJsCssFile", $"{Constant.ScriptPrefix}All.js", "js");
            return services;
        }
    }
}
