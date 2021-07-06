using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cutec.Blazor.WebAPIs.WebAssemblyDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddGeolocation()
                .AddWebStorage()
                .AddIndexedDB<DbContext>();
            
            var host = builder.Build();

            await host.Services.UseGeolocationAsync();

            await host.Services.UseIndexedDbAsync<DbContext>(options =>
            {
                options.Name = "Test";

                // increase the Version number and uncomment DbContext.Tasks to test schema upgrade
                options.Version = 3;
            });

            await host.RunAsync();
        }
    }
}
