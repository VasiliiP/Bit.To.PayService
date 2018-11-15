using System;

namespace Bit.To.SampleEmailSending
{
    public class GetEmailItem : Query<EmailDto>
    {
        public Guid EmailId { get; set; }
    }
}