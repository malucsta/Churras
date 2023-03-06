using Domain.Bbqs.UseCases;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serverless_Api.Bbqs.Functions.ShoppingList;
using Serverless_Api.Bbqs.Validators;

namespace Serverless_Api.Extensions.Validators
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<NewBbqRequest>, NewBbqRequestValidator>();
            services.AddTransient<IValidator<PriceRequest>, PriceRequestValidator>();

            return services;
        }
    }
}
