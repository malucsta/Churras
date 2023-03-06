using Domain.Common.Errors;
using FluentResults;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Serverless_Api.Extensions.ErrorTreatment
{
    public static class FluentResultErrorExtension
    {
        private static ILogger Logger { get => StaticLoggerFactory.GetStaticLogger<Program>(); }

        public static (HttpStatusCode StatusCode, object? Data) ToObjectResult(this List<IError> errors)
        {
            if (errors.First().GetType().IsSubclassOf(typeof(BarbecueError)))
            {
                BarbecueError error = (BarbecueError)errors.First();
                
                dynamic result = new ExpandoObject();

                result.Code = error.Code;
                result.Message = error.Message;
                result.Data = error.Metadata?.DictionaryToObject();

                Logger.LogError($"@@@@ Error: {JsonSerializer.Serialize(result)}");

                if (error.Code == BarbecueErrorCode.RESOURCE_not_found)
                    return (HttpStatusCode.NotFound, null);

                if (error.Code == BarbecueErrorCode.RESOURCE_conflict)
                    return (HttpStatusCode.Conflict, null);

                if (error.Code == BarbecueErrorCode.OPERATION_unauthorized)
                    return (HttpStatusCode.Unauthorized, null);

                return (HttpStatusCode.BadRequest, null);
            }
            else
            {
                return (HttpStatusCode.InternalServerError, null);
            }
        }

        public static object ToInvalidRequestObject(this ValidationResult result)
        {
            var errors = new Dictionary<string, string>();

            foreach (var error in result.Errors)
                errors.Add(error.PropertyName, error.ErrorMessage);

            return errors;
        }

        private static dynamic? DictionaryToObject(this Dictionary<string, object> dict)
        {
            if (dict.Count == 0)
                return null;

            IDictionary<string, object> eo = new ExpandoObject()!;
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                eo.Add(kvp);
            }

            return eo;
        }
    }
}
