namespace Bit.To.SampleEmailSending
{
    public interface IEmailClient
    {
        bool Send(EmailItem mail);
    }
}
