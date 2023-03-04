using Domain.Bbqs.UseCases;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Serverless_Api.Extensions.ErrorTreatment;
using System.Net;

namespace Serverless_Api
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
            var input = await req.Body<ModerateBbqRequest>();
            
            if (input is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            
            input!.Id = id;

            var result = await _useCase.Execute(input);

            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }

            return await req.CreateResponse(HttpStatusCode.OK, result.Value);
        }
    }
}
