using System;
using Bit.Persistence;
using Bit.Persistence.Dapper;
using Dapper.Contrib.Extensions;

namespace Bit.To.SampleEmailSending
{
    public class EmailItemRepository : IEmailItemRepository
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly EmailItemsDbContext.Mapper _mapper;

        public EmailItemRepository(IConnectionFactory connectionFactory, EmailItemsDbContext.Mapper mapper)
        {
            _connectionFactory = connectionFactory;
            _mapper = mapper;
        }

        public EmailItem Find(long id)
        {
            using (var connection = _connectionFactory.Create())
            {
                var dto = connection.Get<EmailItemsDbContext.Dto>(id);
                return dto == null ? null : _mapper.Convert(dto);
            }
        }

        public EmailItem FindByKey(Guid key)
        {
            using (var connection = _connectionFactory.Create())
            {
                var dto = connection.QueryFirstOrDefault(EmailItemsDbContext.SelectById(key));
                return dto == null ? null : _mapper.Convert(dto);
            }
        }

        public void Save(EmailItem item)
        {
            using (var connection = _connectionFactory.Create())
            {
                var dto = _mapper.Convert(item);
                if (!item.HasId)
                {
                    connection.Insert(dto);
                    item.Id = dto.id;
                }
                else
                    connection.Update(dto);
            }
        }
    }
}