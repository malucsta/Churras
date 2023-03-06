using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.People;
using Domain.Bbqs.UseCases;
using Serverless_Api.Extensions.ErrorTreatment;
using FluentValidation;
using FluentValidation.Results;

namespace Serverless_Api.Bbqs.Functions.CreateNewBbq
{
    public partial class RunCreateNewBbq
    {
        private readonly Person _user;
        private readonly ICreateBbq _useCase;
        private readonly IValidator<NewBbqRequest> _validator;

        public RunCreateNewBbq(Person user, ICreateBbq useCase, IValidator<NewBbqRequest> validator)
        {
            _user = user;
            _useCase = useCase;
            _validator = validator;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var request = await req.Body<NewBbqRequest>();

            if (request is null)
                return await req.CreateResponse(HttpStatusCode.BadRequest, "request is required.");

            ValidationResult validationResult = await _validator.ValidateAsync(request!);
            if (!validationResult.IsValid)
                return await req.CreateResponse(HttpStatusCode.BadRequest, validationResult.ToInvalidRequestObject());

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
