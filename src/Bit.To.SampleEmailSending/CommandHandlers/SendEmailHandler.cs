using Bit.To.PersonData;

namespace Bit.To.SampleEmailSending
{
    public class SendEmailHandler : ICommandHandler<SendEmail>
    {
        private readonly IEmailClient _client;
        private readonly IEmailItemRepository _repository;

        public SendEmailHandler(IEmailItemRepository repository, IEmailClient client)
        {
            _repository = repository;
            _client = client;
        }

        public void Execute(SendEmail cmd)
        {
            var item = EmailItem.CreateNew(cmd.Id, Email.Parse(cmd.To), cmd.HtmlBody, cmd.Subject);
            _repository.Save(item);

            var success = _client.Send(item);
            item.SetSendStatus(success);

            _repository.Save(item);
        }
    }
}