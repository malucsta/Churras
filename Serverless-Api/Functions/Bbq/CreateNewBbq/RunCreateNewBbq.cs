using Eveneum;
using System.Net;
using CrossCutting;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Common;
using Domain.Bbqs.Events;
using Domain.Bbqs;
using Domain.People.Events;
using Domain.Lookups;
using Domain.People;
using Domain.Bbqs.UseCases;
using Serverless_Api.Extensions.ErrorTreatment;

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
            var input = await req.Body<NewBbqRequest>();

            if (input is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            
            var result = await _useCase.CreateBbqAsync(input);

            if (result.IsFailed)
            {
                var objectResult = result.Errors.ToObjectResult();
                return await req.CreateResponse(objectResult.StatusCode, objectResult.Data);
            }
                
            return await req.CreateResponse(HttpStatusCode.Created, result.Value);
        }
    }
}
