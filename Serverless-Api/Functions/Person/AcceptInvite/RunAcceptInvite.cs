using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.People;
using Domain.People.Repositories;
using Domain.People.Events;
using Domain.Bbqs.Repositories;
using Azure.Core;
using System.Net;
using FluentResults;
using Domain.People.UseCases;
using Serverless_Api.Extensions.ErrorTreatment;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly Person _user;
        private readonly IAcceptInvite _useCase;
        public RunAcceptInvite(Person user, IAcceptInvite useCase)
        {
            _user = user;
           _useCase = useCase;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<InviteAnswer>();
            if (answer is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "answer is required.");

            answer.InviteId = inviteId;
            answer.UserId = _user.Id;

            var result = await _useCase.Execute(answer);
            
            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }

            return await req.CreateResponse(HttpStatusCode.OK, result.Value);
        }
    }
}
