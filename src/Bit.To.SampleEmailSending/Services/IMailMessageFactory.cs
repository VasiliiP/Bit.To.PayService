using System.Net.Mail;

namespace Bit.To.SampleEmailSending
{
    public interface IMailMessageFactory
    {
        MailMessage Create(string to, string subject, string htmlBody);
    }
}