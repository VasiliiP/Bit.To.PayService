using System.Net.Mail;

namespace Bit.To.SampleEmailSending
{
    public class MailMessageFactory : IMailMessageFactory
    {
        private readonly string _fromAddress;
        private readonly string _fromDisplayName;

        public MailMessageFactory(string fromAddress, string fromDisplayName)
        {
            _fromAddress = fromAddress;
            _fromDisplayName = fromDisplayName;
        }

        public MailMessage Create(string to, string subject, string htmlBody)
        {
            var fromAddress = new MailAddress(_fromAddress, _fromDisplayName);
            var toAddress = new MailAddress(to);
            var mail = new MailMessage(fromAddress, toAddress);

            mail.Headers.Add("Disposition-Notification-To", _fromAddress);
            mail.Headers.Add("X-Confirm-Reading-To", _fromAddress);
            mail.Headers.Add("Return-Receipt-To", _fromAddress);
            mail.Subject = subject;
            mail.Body = htmlBody;
            mail.IsBodyHtml = true;

            return mail;
        }
    }
}