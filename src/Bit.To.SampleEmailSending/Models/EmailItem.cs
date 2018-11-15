using System;
using Bit.Domain;
using Bit.To.PersonData;

namespace Bit.To.SampleEmailSending
{
    public class EmailItem : Entity<long>
    {
        public EmailItem(
            Guid key,
            DateTime createdAt,
            EmailState state,
            DateTime? deliveredAt,
            string text,
            Email dstAddress,
            string subject)
        {
            Key = key;
            CreatedAt = createdAt;
            State = state;
            DeliveredAt = deliveredAt;
            Text = text;
            DstAddress = dstAddress;
            Subject = subject;
        }

        public static EmailItem CreateNew(Guid key, Email dstAddress, string text, string subject)
        {
            return new EmailItem(key, DateTime.Now, EmailState.Created, null, text ?? "", dstAddress, subject);
        }

        public Guid Key { get; }
        public DateTime CreatedAt { get; }
        public DateTime? DeliveredAt { get; private set; }
        public EmailState State { get; private set; }
        public string Text { get; }
        public Email DstAddress { get; }
        public string Subject { get; }

        public void SetSendStatus(bool success)
        {
            State = success ? EmailState.Sended : EmailState.SendError;
        }

        public void SetDeliveryStatus(DateTime date, bool success)
        {
            DeliveredAt = date;
            State = success ? EmailState.Delivered : EmailState.DeliveryFailed;
        }
    }
}