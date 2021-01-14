using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Fluxor;

namespace Anagramimation.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


            builder.Services.AddScoped<Feature>();

            builder.Services.AddFluxor(options =>
                {
                    options.ScanAssemblies(Assembly.GetExecutingAssembly(), typeof(State).Assembly);
                    options.UseReduxDevTools();

                }
            );

            builder.Services.AddAntDesign();

            await builder.Build().RunAsync();
        }
    }
}
