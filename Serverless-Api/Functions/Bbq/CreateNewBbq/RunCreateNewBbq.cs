using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.People;
using Domain.Bbqs.UseCases;
using Serverless_Api.Extensions.ErrorTreatment;
using System.ComponentModel.DataAnnotations;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly Person _user;
        private readonly ICreateBbq _useCase;

        public RunCreateNewBbq(Person user, ICreateBbq useCase)
        {
            _user = user;   
            _useCase = useCase;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var request = await req.Body<NewBbqRequest>();

            //ValidationResult result = await _createRequestValidator.ValidateAsync(request);
            //if (!result.IsValid)
            //    return BadRequest(result);

            if (request is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "request is required.");
            
            var result = await _useCase.CreateBbqAsync(request);

            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }
                
            return await req.CreateResponse(HttpStatusCode.Created, result.Value);
        }
    }
}
