using System.Collections.Generic;
using MessengerServer.Domain.Models;

namespace MessengerServer.Domain.Repositories
{
    public interface IContactRepository
    {
        ContactDataModel GetUserContact(int userId, int contactId);
        IEnumerable<ContactDataModel> GetUserContacts(int userId);
        IEnumerable<ContactDataModel> SearchUserContacts(int userId, string query);
        void AddContact(ContactDataModel contact);
        void UpdateContact(ContactDataModel updatedContact);
        void DeleteContact(int userId, int contactId);
    }
}