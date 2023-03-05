using Domain.Common;
using Domain.People;
using Domain.People.Repositories;
using Domain.People.UseCases;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Serverless_Api.Extensions.ErrorTreatment;

namespace Serverless_Api
{
    public partial class RunGetInvites
    {
        private readonly Person _user;
        private readonly IGetInvites _useCase;

        public RunGetInvites(Person user, IGetInvites useCase)
        {
            _user = user;
            _useCase = useCase;
        }

        [Function(nameof(RunGetInvites))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/invites")] HttpRequestData req)
        {
            var request = new GetInvitesRequest { UserId = _user.Id };
            
            var result = await _useCase.Execute(request);

            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, result.Value);
        }
    }
}
