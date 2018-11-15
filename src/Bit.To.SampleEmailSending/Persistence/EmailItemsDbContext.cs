using System;
using Bit.Persistence;
using Bit.To.PersonData;
using Dapper.Contrib.Extensions;

namespace Bit.To.SampleEmailSending
{
    public class EmailItemsDbContext
    {
        private const string Table = "emails";

        [Table(Table)]
        public class Dto
        {
            public long id { get; set; }
            public Guid email_key { get; set; }
            public DateTime created_at { get; set; }
            public int email_state { get; set; }
            public DateTime? delivered_at { get; set; }
            public string email_text { get; set; }
            public string dst_address { get; set; }
            public string email_subj { get; set; }
        }

        public static QueryObject<Dto> SelectById(Guid key)
        {
            return new QueryObject<Dto>($"SELECT * FROM {Table} WHERE email_key=@key", new {key});
        }

        public class Mapper
        {
            public Dto Convert(EmailItem item)
            {
                return new Dto
                {
                    id = item.Id,
                    created_at = item.CreatedAt,
                    delivered_at = item.DeliveredAt,
                    email_key = item.Key,
                    email_state = (int) item.State,
                    email_text = item.Text,
                    dst_address = item.DstAddress.Address,
                    email_subj = item.Subject,
                };
            }

            public EmailItem Convert(Dto dto)
            {
                return new EmailItem(
                    dto.email_key,
                    dto.created_at,
                    (EmailState) dto.email_state,
                    dto.delivered_at,
                    dto.email_text,
                    Email.Parse(dto.dst_address),
                    dto.email_subj) {Id = dto.id};
            }
        }
    }
}