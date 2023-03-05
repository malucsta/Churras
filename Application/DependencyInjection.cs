using Application.UseCases.Bbqs;
using Application.UseCases.People;
using Domain.Bbqs.UseCases;
using Domain.People.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection InjectUseCases(this IServiceCollection services)
        {
            services.AddScoped<ICreateBbq, CreateBbq>();
            services.AddScoped<IModerateBbq, ModerateBbq>();
            services.AddScoped<IListBbqs, ListBbqs>();
            services.AddScoped<IGetShoppingList, GetShoppingList>();

            services.AddScoped<IAcceptInvite, AcceptInvite>();
            services.AddScoped<IDeclineInvite, DeclineInvite>();
            services.AddScoped<IGetInvites, GetInvites>();

            return services;
        }
    }
}
