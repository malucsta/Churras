using Domain.Bbqs.UseCases;
using Domain.People;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using Serverless_Api.Extensions.ErrorTreatment;
using FluentValidation;
using FluentValidation.Results;

namespace Serverless_Api.Bbqs.Functions.ShoppingList
{
    public partial class RunGetShoppingListEstimate
    {
        private readonly Person _user;
        private readonly IGetShoppingListEstimate _useCase;
        private readonly IValidator<PriceRequest> _validator;

        public RunGetShoppingListEstimate(Person user, IGetShoppingListEstimate useCase, IValidator<PriceRequest> validator)
        {
            _user = user;
            _useCase = useCase;
            _validator = validator;
        }

        [Function(nameof(RunGetShoppingListEstimate))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{bbqId}/lista-de-compras/estimativa")] HttpRequestData req, string bbqId)
        {
            var priceRequest = await req.Body<PriceRequest>();

            if (priceRequest is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "request is required.");

            ValidationResult validationResult = await _validator.ValidateAsync(priceRequest!);
            if (!validationResult.IsValid)
                return await req.CreateResponse(HttpStatusCode.BadRequest, validationResult.ToInvalidRequestObject());

            var request = new GetShoppingListEstimateRequest
            {
                BbqId = bbqId,
                UserId = _user.Id,
                MeatPricePerKg = priceRequest.MeatPricePerKg,
                VegetablesPricePerKg = priceRequest.VegetablesPricePerKg
            };

            var result = await _useCase.Execute(request);

            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }

            return await req.CreateResponse(HttpStatusCode.OK, result.Value);
        }
    }
}
