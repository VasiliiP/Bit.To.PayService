namespace Bit.To.SampleEmailSending
{
    public class GetEmailItemHandler : IQueryHandler<GetEmailItem, EmailDto>
    {
        private readonly IEmailItemRepository _repository;

        public GetEmailItemHandler(IEmailItemRepository repository)
        {
            _repository = repository;
        }

        public EmailDto Execute(GetEmailItem query)
        {
            var item = _repository.FindByKey(query.EmailId);
            if (item == null) return null;

            return new EmailDto
            {
                CreatedAt = item.CreatedAt,
                DeliveredAt = item.DeliveredAt,
                To = item.DstAddress.ToString(),
                HtmlBody = item.Text,
                State = item.State,
                Subject = item.Subject,
            };
        }
    }
}