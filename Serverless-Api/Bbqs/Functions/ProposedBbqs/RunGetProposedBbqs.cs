using System.Net;
using Domain.Bbqs.UseCases;
using Domain.People;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Serverless_Api.Extensions.ErrorTreatment;

namespace Serverless_Api.Bbqs.Functions.ProposedBbqs
{
    public partial class RunGetProposedBbqs
    {
        private readonly Person _user;
        private readonly IListBbqs _useCase;

        public RunGetProposedBbqs(Person user, IListBbqs useCase)
        {
            _user = user;
            _useCase = useCase;
        }

        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {
            var request = new GetProposedBbqsRequest { UserId = _user.Id };
            if (request is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "request is required.");

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
