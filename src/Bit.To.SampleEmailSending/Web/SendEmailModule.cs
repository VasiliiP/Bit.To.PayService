using System;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;

namespace Bit.To.SampleEmailSending
{
    public sealed class SendEmailModule : NancyModule
    {
        public SendEmailModule(ICommandHandler<SendEmail> handler, string modulePath = "/") : base(modulePath)
        {
            Post(
                "{id}/",
                param =>
                {
                    var cmd = CreateCmd(Context, param);
                    if (cmd == null)
                        return HttpStatusCode.BadRequest;

                    handler.Execute(cmd);
                    
                    return new ResourceCreatedResponse(Request.Url.BasePath.ToString(), cmd.Id.ToString());
                },
                null,
                nameof(SendEmailModule)+"_id");

            Post(
                "/",
                _ =>
                {
                    var cmd = CreateCmd(Context);
                    if (cmd == null)
                        return HttpStatusCode.BadRequest;

                    handler.Execute(cmd);
                    return new ResourceCreatedResponse(Request.Url.ToString(), cmd.Id.ToString());
                },
                null,
                nameof(SendEmailModule));
        }

        private static SendEmail CreateCmd(NancyContext context, dynamic param)
        {
            var cmd = CreateCmd(context);
            if (cmd == null)
                return null;

            if (!Guid.TryParse((string) param["id"], out var id) || id == Guid.Empty)
                return null;

            cmd.Id = id;
            return cmd;
        }

        private static SendEmail CreateCmd(NancyContext context)
        {
            var s = context.Request.Body.AsString();
            if (String.IsNullOrWhiteSpace(s)) return null;
            var cmd = JsonConvert.DeserializeObject<SendEmail>(s);
            cmd.Id = Guid.NewGuid();
            return cmd;
        }
    }
}