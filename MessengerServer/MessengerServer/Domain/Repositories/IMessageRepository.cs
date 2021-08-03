using System.Collections.Generic;
using MessengerServer.Domain.Models;

namespace MessengerServer.Domain.Repositories
{
    public interface IMessageRepository
    {
        IEnumerable<MessageDataModel> GetUserMessages(int userId);
        IEnumerable<MessageDataModel> SearchUserMessages(int userId, int contactId, string query);
        void AddMessage(MessageDataModel message);
    }
}