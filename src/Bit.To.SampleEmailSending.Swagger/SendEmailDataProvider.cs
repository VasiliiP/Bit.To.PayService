using Nancy.Swagger;
using Nancy.Swagger.Services;

namespace Bit.To.SampleEmailSending
{
    public class SendEmailDataProvider : ISwaggerModelDataProvider
    {
        public SwaggerModelData GetModelData()
        {
            return SwaggerModelData.ForType<SendEmail>(
                with =>
                {
                    with.Description("Команда отправки почты");
                    with.Property(x => x.Id)
                        .Description("Не используется")
                        .Required(false);
                    with.Property(x => x.To)
                        .Description("Адрес назначения")
                        .Required(true);
                    with.Property(x => x.Subject)
                        .Description("Тема письма")
                        .Required(false);
                    with.Property(x => x.HtmlBody)
                        .Description("Тело письма в HTML-разметке")
                        .Required(false);
                });
        }
    }
}