using System;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace Bit.To.SampleEmailSending
{
    public class GetEmailItemRestClient : IQueryHandler<GetEmailItem, EmailDto>
    {
        private readonly string _baseUrl;
        
        public GetEmailItemRestClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public EmailDto Execute(GetEmailItem query)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"{query.EmailId}/", Method.GET);
            
            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(response.StatusDescription);

            var dto = JsonConvert.DeserializeObject<EmailDto>(response.Content);
            return dto;
        }
    }
}
