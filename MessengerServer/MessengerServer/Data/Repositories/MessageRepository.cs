using System;
using System.Collections.Generic;
using System.Linq;
using MessengerServer.Domain.Models;
using MessengerServer.Domain.Repositories;

namespace MessengerServer.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMessengerContext _context;

        public MessageRepository(IMessengerContext context)
        {
            _context = context;
        }

        public IEnumerable<MessageDataModel> GetUserMessages(int userId)
        {
            return _context.MessageDataModels.Where(m => m.UserId == userId);
        }

        public IEnumerable<MessageDataModel> SearchUserMessages(int userId, int contactId, string query)
        {
            return _context.MessageDataModels
                .Where(m =>
                    (m.UserId == userId && m.ContactId == contactId ||
                     m.UserId == contactId && m.ContactId == userId) &&
                    m.Content.ToLower().Contains(query.ToLower()));
        }

        public void AddMessage(MessageDataModel message)
        {
            ValidateMessage(message);
            _context.MessageDataModels.Add(message);
            _context.SaveChanges();
        }

        private static void ValidateMessage(MessageDataModel message)
        {
            if (message.UserId == message.ContactId)
                throw new ArgumentException("Can't message yourself");
        }
    }
}