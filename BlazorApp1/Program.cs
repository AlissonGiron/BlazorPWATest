using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorApp1.Services;
using Blazored.LocalStorage;

namespace BlazorApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredLocalStorage(config =>
                config.JsonSerializerOptions.WriteIndented = true);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<IProductService, ProductService>();

            await builder.Build().RunAsync();
        }
    }
}
