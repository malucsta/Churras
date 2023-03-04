using Application.UseCases.Bbqs;
using Domain.Bbqs.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectUseCases(this IServiceCollection services)
        {
            services.AddScoped<ICreateBbq, CreateBbq>();
            services.AddScoped<IGetShoppingList, GetShoppingList>();

            return services;
        }
    }
}
