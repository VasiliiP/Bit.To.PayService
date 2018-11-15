using System.Linq;
using Nancy;

namespace Bit.To.SampleEmailSending
{
    public class ResourceCreatedResponse : Response
    {
        public ResourceCreatedResponse(string resourceLocation, string resourceId)
        {
            StatusCode = HttpStatusCode.Created;
            var delimiter = resourceLocation.Last() != '/'
                ? "/"
                : "";

            Headers.Add("Location", resourceLocation + delimiter + resourceId);
        }
    }
}