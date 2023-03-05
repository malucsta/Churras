using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.People;
using Domain.People.Repositories;
using Domain.People.Events;
using Domain.Bbqs.Repositories;
using Domain.People.UseCases;
using Serverless_Api.Extensions.ErrorTreatment;

namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly Person _user;
        private readonly IDeclineInvite _useCase; 

        public RunDeclineInvite(Person user, IDeclineInvite useCase)
        {
            _user = user;
            _useCase = useCase;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var answer = new DeclineAnswer { UserId = _user.Id, InviteId = inviteId };
            
            var result = await _useCase.Execute(answer);

            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, result.Value);
        }
    }
}
