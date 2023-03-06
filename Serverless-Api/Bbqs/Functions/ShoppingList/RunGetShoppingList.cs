using System.Net;
using Domain.Bbqs;
using Domain.Bbqs.Repositories;
using Domain.Bbqs.UseCases;
using Domain.People;
using Domain.People.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Serverless_Api.Extensions.ErrorTreatment;

namespace Serverless_Api.Bbqs.Functions.ShoppingList
{
    public partial class RunGetShoppingList
    {
        private readonly Person _user;
        private readonly IGetShoppingList _useCase;
        public RunGetShoppingList(Person user, IGetShoppingList useCase)
        {
            _user = user;
            _useCase = useCase;
        }

        [Function(nameof(RunGetShoppingList))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{bbqId}/lista-de-compras")] HttpRequestData req, string bbqId)
        {
            var result = await _useCase.Execute(new GetShoppingListRequest { Id = bbqId, UserId = _user.Id });

            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }

            return await req.CreateResponse(HttpStatusCode.OK, result.Value);
        }
    }
}
