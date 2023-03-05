using Domain.Bbqs.UseCases;
using Domain.People;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using Serverless_Api.Extensions.ErrorTreatment;

namespace Serverless_Api.Functions.Bbq.ShoppingList
{
    public partial class RunGetShoppingListEstimate
    {
        private readonly Person _user;
        private readonly IGetShoppingListEstimate _useCase;
        
        public RunGetShoppingListEstimate(Person user, IGetShoppingListEstimate useCase)
        {
            _user = user;
            _useCase = useCase;
        }

        [Function(nameof(RunGetShoppingListEstimate))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{bbqId}/lista-de-compras/estimativa")] HttpRequestData req, string bbqId)
        {
            var priceRequest = await req.Body<PriceRequest>();

            if (priceRequest is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "request is required.");

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
