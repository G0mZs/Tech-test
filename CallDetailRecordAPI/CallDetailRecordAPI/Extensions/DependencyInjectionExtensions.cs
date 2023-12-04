using CallDetailRecordAPI.Services;
using CallDetailRecordAPI.Services.Interfaces;

namespace CallDetailRecordAPI.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICdrService, CdrService>();

            return services;
        }
    }
}
