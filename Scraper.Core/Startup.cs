using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scraper.Core.Data;
using Scraper.Core.Services;

namespace Scraper.Core
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("ScraperDb")));

            services.AddMemoryCache();

            services.AddScoped<SourceService>();
            services.AddScoped<ScrapDetailService>();
            services.AddScoped<HtmlParserService>();

            services.AddSingleton<IPlaywrightService, PlaywrightService>();
        }
    }
}