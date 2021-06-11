using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs
{
    public static partial class ServicesExtension
    {
        public static IServiceCollection AddIndexedDB<TIndexedDb>(this IServiceCollection services) where TIndexedDb: IndexedDb
        {
            services.AddSingleton<TIndexedDb>();
            return services;
        }

        public static async Task<IServiceProvider> UseIndexedDbAsync<TIndexedDb>(this IServiceProvider services, Action<IndexedDbOptions> configureOptions) where TIndexedDb : IndexedDb
        {
            var options = new IndexedDbOptions();
            configureOptions(options);

            if (string.IsNullOrWhiteSpace(options.Name))
            {
                throw new Exception("IndexedDbOptions.Name is required.");
            }

            var db = services.GetRequiredService<TIndexedDb>();
            var js = services.GetRequiredService<IJSRuntime>();

            bool loaded;

            try
            {
                loaded = await js.InvokeAsync<bool>($"{Constant.JsAgent}.getLoadedFlag", nameof(IndexedDb));
            }
            catch
            {
                loaded = false;
            }
            
            if (!loaded)
            {
                await js.InvokeVoidAsync($"{Constant.JsAgent}.loadJsCssFile", $"{Constant.ScriptPrefix}IndexedDb.js", "js");
            }

            await db.InitializeAsync(options, js);

            return services;
        }
    }
}
