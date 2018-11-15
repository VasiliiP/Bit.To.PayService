using System;

namespace Bit.To.SampleEmailSending
{
    public class EmailDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public EmailState State { get; set; }
    }
}