using Blazored.LocalStorage;
using InHues.StateMngmt.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace InHues.StateMngmt
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddStateMngmt(this IServiceCollection services) {
            services.AddBlazoredLocalStorage();
            services.AddScoped<IStorageMngmt, StorageMngmt>();
            services.AddScoped<AppState>();
            return services;
        }
    }
}
