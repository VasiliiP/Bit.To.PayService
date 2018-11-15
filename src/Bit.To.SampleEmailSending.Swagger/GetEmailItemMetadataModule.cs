using Nancy;
using Nancy.Metadata.Modules;
using Nancy.Swagger;
using Swagger.ObjectModel;

namespace Bit.To.SampleEmailSending
{
    public sealed class GetEmailItemMetadataModule : MetadataModule<PathItem>
    {
        public GetEmailItemMetadataModule(ISwaggerModelCatalog modelCatalog)
        {
            modelCatalog.AddModel<EmailDto>();
            modelCatalog.AddModel<EmailState>();
            
            Describe[nameof(GetEmailItemModule)] = description => description.AsSwagger(
                with => with.Operation(
                    op => op.OperationId(nameof(GetEmailItem))
                        .Tag(nameof(GetEmailItem))
                        .Summary("Получить информацию о email-сообщении")
                        .Parameter(p => p.In(ParameterIn.Path).Name("id").Description("id сообщения"))
                        .Response(
                            (int)HttpStatusCode.NotFound, r => r.Description("Email с таким id не найден"))
                        .Response(
                            (int)HttpStatusCode.OK,
                            r => r.Description("Email-сообщение").Schema<EmailDto>())
                ));
        }
    }
}