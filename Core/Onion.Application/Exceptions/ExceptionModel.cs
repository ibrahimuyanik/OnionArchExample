using Newtonsoft.Json;

namespace Onion.Application.Exceptions
{
    public class ExceptionModel: ErrorStatusCode
    {
        public IEnumerable<string> Errors { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
            // parametre de this yazarak bu class'ın prop'larını serialize etmesi gerektiğini belirttik
        }
    }

    public class ErrorStatusCode
    {
        public int StatusCode { get; set; }
    }
}
