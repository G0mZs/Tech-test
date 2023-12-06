using CallDetailRecordAPI.Services;
using CallDetailRecordAPI.Services.Interfaces;

namespace CallDetailRecordAPI.Extensions
{
    /// <summary>Represents the dependency injection extensions.</summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>Adds the services.</summary>
        /// <param name="services">The services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICdrService, CdrService>();

            return services;
        }
    }
}
