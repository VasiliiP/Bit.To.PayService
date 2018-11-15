using Bit.To.EmailSending.Logging;

namespace Bit.To.SampleEmailSending
{
    public class FakeSmtpClient : IEmailClient
    {
        private readonly string _password;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly IMailMessageFactory _factory;
        private readonly string _userName;
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public FakeSmtpClient(string userName, string password, string smtpHost, int smtpPort, IMailMessageFactory factory)
        {
            _userName = userName;
            _password = password;
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _factory = factory;
        }

        public bool Send(EmailItem emailItem)
        {
            var mail = _factory.Create(emailItem.DstAddress.Address, emailItem.Subject, emailItem.Text);
            Log.TraceFormat("Email {@mail} sended. Host: {_smtpHost} port: {_smtpPort}", mail, _smtpHost, _smtpPort);
            return true;
        }
    }
}
