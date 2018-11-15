using System;

namespace Bit.To.SampleEmailSending
{
    public interface IEmailItemRepository
    {
        EmailItem Find(long id);
        EmailItem FindByKey(Guid key);
        void Save(EmailItem item);
    }
}