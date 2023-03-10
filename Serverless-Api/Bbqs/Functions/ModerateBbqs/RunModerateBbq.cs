using Domain.Bbqs.UseCases;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Serverless_Api.Extensions.ErrorTreatment;
using System.Net;

namespace Serverless_Api.Bbqs.Functions.ModerateBbqs
{
    public partial class RunModerateBbq
    {
        private readonly IModerateBbq _useCase;
        public RunModerateBbq(IModerateBbq useCase)
        {
            _useCase = useCase;
        }

        [Function(nameof(RunModerateBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            var request = await req.Body<ModerateBbqRequest>();

            if (request is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "request is required.");            

            request!.Id = id;

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
