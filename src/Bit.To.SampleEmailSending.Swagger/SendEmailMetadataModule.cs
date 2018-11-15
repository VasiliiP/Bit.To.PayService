using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;

namespace Bit.To.SampleEmailSending
{
    public sealed class SendEmailMetadataModule : MetadataModule<PathItem>
    {
        public SendEmailMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<SendEmail>();
            
            Describe[nameof(SendEmailModule)] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.OperationId(nameof(SendEmail))
                        .Tag(nameof(SendEmail))
                        .Summary("Отправка email-сообщения")
                        .ConsumeMimeType("application/json")
                        .BodyParameter(p => p.Name("body").Schema<SendEmail>())
                        .Response(
                            (int)HttpStatusCode.UnprocessableEntity, r => r.Description("Невалидные данные"))
                        .Response(
                            (int)HttpStatusCode.Created,
                            r => r.Description("Успех").Header(
                                "Location",
                                new Header { Description = description.Path + "{id}" }))
                ));
            
            Describe[nameof(SendEmailModule)+"_id"] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.OperationId(nameof(SendEmail)+"_id")
                        .Tag(nameof(SendEmail))
                        .Summary("Отправка email-сообщения (с указанием id)")
                        .Parameter(p => p.In(ParameterIn.Path).Name("id").Description("id сообщения"))
                        .ConsumeMimeType("application/json")
                        .BodyParameter(p => p.Name("body").Schema<SendEmail>())
                        .Response(
                            (int)HttpStatusCode.UnprocessableEntity, r => r.Description("Невалидные данные"))
                        .Response(
                            (int)HttpStatusCode.Created,
                            r => r.Description("Успех"))
                ));
        }
    }
}
