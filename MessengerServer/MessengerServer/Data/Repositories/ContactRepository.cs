using System;
using System.Collections.Generic;
using System.Linq;
using MessengerServer.Domain.Models;
using MessengerServer.Domain.Repositories;

namespace MessengerServer.Data.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IMessengerContext _context;
        private readonly IUserRepository _userRepository;

        public ContactRepository(IMessengerContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public ContactDataModel GetUserContact(int userId, int contactId)
        {
            return _context.ContactDataModels.FirstOrDefault(c =>
                c.UserId == userId &&
                c.ContactId == contactId);
        }

        public IEnumerable<ContactDataModel> GetUserContacts(int userId)
        {
            return _context.ContactDataModels.Where(c => c.UserId == userId);
        }

        public IEnumerable<ContactDataModel> SearchUserContacts(int userId, string query)
        {
            var contacts = _context.ContactDataModels
                .Where(c => c.UserId == userId)
                .ToList();

            return contacts.Where(contact => ContactNameContainsQuery(contact, query));
        }

        public void AddContact(ContactDataModel contact)
        {
            ValidateContact(contact);
            _context.ContactDataModels.Add(contact);
            _context.SaveChanges();
        }

        public void UpdateContact(ContactDataModel updatedContact)
        {
            ValidateContact(updatedContact, true);
            var contact = _context.ContactDataModels.FirstOrDefault(c =>
                c.ContactId == updatedContact.ContactId &&
                c.UserId == updatedContact.UserId);
            if (contact is null) throw new ArgumentException("Contact is not found");
            contact.LastUpdateTime = updatedContact.LastUpdateTime;
            _context.SaveChanges();
        }

        public void DeleteContact(int userId, int contactId)
        {
            var contact = _context.ContactDataModels.FirstOrDefault(c =>
                c.UserId == userId &&
                c.ContactId == contactId);
            if (contact is null) throw new ArgumentException("Contact is not found");

            _context.ContactDataModels.Remove(contact);
            _context.SaveChanges();
        }

        private bool ContactNameContainsQuery(ContactDataModel contact, string query)
        {
            var contactUser = _userRepository.GetUserById(contact.ContactId);
            return contactUser.Name.ToLower().Contains(query.ToLower());
        }

        private void ValidateContact(ContactDataModel contact, bool isUpdate = false)
        {
            if (!isUpdate && _context.ContactDataModels.Any(c =>
                c.ContactId == contact.ContactId &&
                c.UserId == contact.UserId))
                throw new ArgumentException("This contact already exist");
            if (contact.UserId == contact.ContactId)
                throw new ArgumentException("You can't contact yourself");
        }
    }
}