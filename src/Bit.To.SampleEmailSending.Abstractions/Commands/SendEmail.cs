using System;

namespace Bit.To.SampleEmailSending
{
    public class SendEmail : Command
    {
        public Guid Id { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
    }
}