using System;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;

namespace Bit.To.SampleEmailSending
{
    public sealed class GetEmailItemModule : NancyModule
    {
        public GetEmailItemModule(IQueryHandler<GetEmailItem, EmailDto> handler, string modulePath = "/") : base(modulePath)
        {
            Get(
                "{id}/",
                param =>
                {
                    var q = CreateQuery(Context, param) as GetEmailItem;
                    if (q == null)
                        return HttpStatusCode.BadRequest;

                    var dto = handler.Execute(q);

                    if (dto == null) return HttpStatusCode.NotFound;
                    return new TextResponse(JsonConvert.SerializeObject(dto), "application/json");
                },
                null,
                nameof(GetEmailItemModule));
        }

        private static GetEmailItem CreateQuery(NancyContext context, dynamic param)
        {
            if (!Guid.TryParse((string) param["id"], out var id) || id == Guid.Empty)
                return null;

            var q = new GetEmailItem {EmailId = id};
            return q;
        }
    }
}