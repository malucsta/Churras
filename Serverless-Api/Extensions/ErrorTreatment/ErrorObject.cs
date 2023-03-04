namespace Serverless_Api.Extensions.ErrorTreatment
{
    public class ErrorObject
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, object>? Data { get; set; }
    }
}
