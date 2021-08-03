using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using MessengerServer.Data.Repositories;
using MessengerServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MessengerServer.UnitTests
{
    public class ContactRepositoryTests : IDisposable
    {
        private readonly ContactRepository _sut;
        private readonly MessengerContext _context;
        private readonly Fixture _fixture;

        public ContactRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MessengerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _fixture = new Fixture();
            _context = new MessengerContext(options);
            _sut = new ContactRepository(_context, new UserRepository(_context));
        }

        [Fact]
        public void AddContact_ShouldAddContact_WhenContactIsValid()
        {
            var contact = _fixture.Create<ContactDataModel>();

            _sut.AddContact(contact);

            _context.ContactDataModels.Should().Contain(c =>
                c.ContactId == contact.ContactId &&
                c.UserId == contact.UserId &&
                c.LastUpdateTime == contact.LastUpdateTime);
        }

        [Fact]
        public void AddContact_ShouldFail_WhenContactAlreadyExist()
        {
            var contact1 = _fixture.Create<ContactDataModel>();
            var contact2 = _fixture.Build<ContactDataModel>()
                .With(c => c.ContactId, contact1.ContactId)
                .With(c => c.UserId, contact1.UserId)
                .Create();
            _sut.AddContact(contact1);

            Action act = () => _sut.AddContact(contact2);

            act.Should().Throw<ArgumentException>().WithMessage("This contact already exist");
        }

        [Fact]
        public void AddContact_ShouldFail_WhenContactingYourself()
        {
            var contact = _fixture.Build<ContactDataModel>()
                .With(c => c.ContactId, 100500)
                .With(c => c.UserId, 100500)
                .Create();

            Action act = () => _sut.AddContact(contact);

            act.Should().Throw<ArgumentException>().WithMessage("You can't contact yourself");
        }

        [Fact]
        public void UpdateContact_ShouldUpdateContact_WhenContactIsValid()
        {
            var contact = _fixture.Create<ContactDataModel>();
            var updatedContact = _fixture.Build<ContactDataModel>()
                .With(c => c.ContactId, contact.ContactId)
                .With(c => c.UserId, contact.UserId)
                .Create();
            _sut.AddContact(contact);

            _sut.UpdateContact(updatedContact);

            _context.ContactDataModels.Should().Contain(c =>
                c.ContactId == updatedContact.ContactId &&
                c.UserId == updatedContact.UserId &&
                c.LastUpdateTime == updatedContact.LastUpdateTime);
        }

        [Fact]
        public void DeleteContact_ShouldDeleteContact_WhenContactExists()
        {
            var contact = _fixture.Create<ContactDataModel>();
            _sut.AddContact(contact);

            _sut.DeleteContact(contact.UserId, contact.ContactId);

            _context.ContactDataModels.Should().NotContain(c =>
                c.ContactId == contact.ContactId &&
                c.UserId == contact.UserId);
        }

        [Fact]
        public void UpdateContact_ShouldFail_WhenContactDoesNotExist()
        {
            var contact = _fixture.Create<ContactDataModel>();

            Action act = () => _sut.UpdateContact(contact);

            act.Should().Throw<ArgumentException>().WithMessage("Contact is not found");
        }

        [Fact]
        public void DeleteContact_ShouldFail_WhenContactDoesNotExist()
        {
            var contact = _fixture.Create<ContactDataModel>();

            Action act = () => _sut.DeleteContact(contact.UserId, contact.ContactId);

            act.Should().Throw<ArgumentException>().WithMessage("Contact is not found");
        }

        [Fact]
        public void GetUserContact_ShouldReturnContact_WhenContactExists()
        {
            var contact = _fixture.Create<ContactDataModel>();
            _sut.AddContact(contact);

            var result = _sut.GetUserContact(contact.UserId, contact.ContactId);

            result.Should().Be(contact);
        }

        [Fact]
        public void GetUserContacts_ShouldReturnContacts_WhenContactsExist()
        {
            const int userId = 9001;
            var contacts = new List<ContactDataModel>()
            {
                _fixture.Create<ContactDataModel>(),
                _fixture.Create<ContactDataModel>(),
                _fixture.Create<ContactDataModel>(),
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, userId)
                    .Create(),
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, userId)
                    .Create(),
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, userId)
                    .Create(),
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, userId)
                    .Create(),
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, userId)
                    .Create()
            };
            foreach (var contact in contacts)
            {
                _sut.AddContact(contact);
            }

            var result = _sut.GetUserContacts(userId);

            result.Should().HaveCount(5);
        }

        [Fact]
        public void GetUserContacts_ShouldReturnNull_WhenContactsDoesNotExist()
        {
            const int userId = 9001;
            var contacts = new List<ContactDataModel>()
            {
                _fixture.Create<ContactDataModel>(),
                _fixture.Create<ContactDataModel>(),
                _fixture.Create<ContactDataModel>()
            };
            foreach (var contact in contacts)
            {
                _sut.AddContact(contact);
            }

            var result = _sut.GetUserContacts(userId);

            result.Should().HaveCount(0);
        }

        [Fact]
        public void SearchUserContacts_ShouldReturnContacts_WhenContactsValidContactsExist()
        {
            var users = new List<UserDataModel>()
            {
                _fixture.Build<UserDataModel>()
                    .With(u => u.Name, "LoveCats")
                    .Create(),
                _fixture.Build<UserDataModel>()
                    .With(u => u.Name, "LoveDogs")
                    .Create(),
                _fixture.Build<UserDataModel>()
                    .With(u => u.Name, "LoveAnimals")
                    .Create(),
                _fixture.Build<UserDataModel>()
                    .With(u => u.Name, "LoveEverything")
                    .Create()
            };
            foreach (var user in users)
            {
                _context.UserDataModels.Add(user);
            }

            _context.SaveChanges();
            var contacts = new List<ContactDataModel>()
            {
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, users[3].UserId)
                    .With(c => c.ContactId, users[2].UserId)
                    .Create(),
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, users[3].UserId)
                    .With(c => c.ContactId, users[1].UserId)
                    .Create(),
                _fixture.Build<ContactDataModel>()
                    .With(c => c.UserId, users[3].UserId)
                    .With(c => c.ContactId, users[0].UserId)
                    .Create()
            };
            foreach (var contact in contacts)
            {
                _sut.AddContact(contact);
            }

            var result = _sut.SearchUserContacts(users[3].UserId, "love");

            result.Should().HaveCount(3);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}