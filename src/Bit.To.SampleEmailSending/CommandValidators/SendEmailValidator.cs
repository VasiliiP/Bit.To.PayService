using Bit.To.PersonData;
using Bit.Validation;

namespace Bit.To.SampleEmailSending
{
    public class SendEmailValidator : IValidator<SendEmail>
    {
        public IValidationResult Validate(SendEmail cmd)
        {
            return Email.Validate(cmd.To);
        }
    }
}