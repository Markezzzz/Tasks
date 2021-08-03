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
    public class MessageRepositoryTests
    {
        private readonly MessageRepository _sut;
        private readonly MessengerContext _context;
        private readonly Fixture _fixture;

        public MessageRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MessengerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _fixture = new Fixture();
            _context = new MessengerContext(options);
            _sut = new MessageRepository(_context);
        }

        [Fact]
        public void AddMessage_ShouldAddMessage_WhenMessageIsValid()
        {
            var message = _fixture.Create<MessageDataModel>();

            _sut.AddMessage(message);

            _context.MessageDataModels.Should().Contain(message);
        }

        [Fact]
        public void AddMessage_ShouldFail_WhenMessageIsInvalid()
        {
            const int userId = 9001;
            var message = _fixture.Build<MessageDataModel>()
                .With(m => m.UserId, userId)
                .With(m => m.ContactId, userId)
                .Create();

            Action act = () => _sut.AddMessage(message);

            act.Should().Throw<ArgumentException>().WithMessage("Can't message yourself");
        }

        [Fact]
        public void GetUserMessages_ShouldReturnMessages_WhenMessagesExist()
        {
            const int userId = 9001;
            var messages = new List<MessageDataModel>()
            {
                _fixture.Build<MessageDataModel>()
                    .With(m => m.UserId, userId)
                    .Create(),
                _fixture.Build<MessageDataModel>()
                    .With(m => m.UserId, userId)
                    .Create(),
                _fixture.Build<MessageDataModel>()
                    .With(m => m.UserId, userId)
                    .Create(),
                _fixture.Create<MessageDataModel>(),
                _fixture.Create<MessageDataModel>(),
                _fixture.Create<MessageDataModel>(),
                _fixture.Create<MessageDataModel>()
            };
            foreach (var message in messages)
            {
                _sut.AddMessage(message);
            }

            var result = _sut.GetUserMessages(userId);

            result.Should().HaveCount(3);
        }

        [Fact]
        public void GetUserMessages_ShouldBeEmpty_WhenNoValidMessagesExist()
        {
            const int userId = 9001;
            var messages = new List<MessageDataModel>()
            {
                _fixture.Create<MessageDataModel>(),
                _fixture.Create<MessageDataModel>(),
                _fixture.Create<MessageDataModel>(),
                _fixture.Create<MessageDataModel>()
            };
            foreach (var message in messages)
            {
                _sut.AddMessage(message);
            }

            var result = _sut.GetUserMessages(userId);

            result.Should().BeEmpty();
        }

        [Fact]
        public void SearchUserMessages_ShouldReturnMessages_WhenValidMessagesExist()
        {
            const int userId = 9001;
            const int contactId = 9002;
            var messages = new List<MessageDataModel>()
            {
                _fixture.Build<MessageDataModel>()
                    .With(m => m.UserId, userId)
                    .With(m => m.ContactId, contactId)
                    .With(m => m.Content, "I Love Cats")
                    .Create(),
                _fixture.Build<MessageDataModel>()
                    .With(m => m.UserId, userId)
                    .With(m => m.ContactId, contactId)
                    .With(m => m.Content, "I Love Dogs")
                    .Create(),
                _fixture.Build<MessageDataModel>()
                    .With(m => m.UserId, userId)
                    .With(m => m.ContactId, contactId)
                    .With(m => m.Content, "I Love Animals")
                    .Create(),
                _fixture.Build<MessageDataModel>()
                    .With(m => m.UserId, userId)
                    .With(m => m.ContactId, contactId)
                    .With(m => m.Content, "I Love Everything")
                    .Create(),
                _fixture.Create<MessageDataModel>(),
                _fixture.Create<MessageDataModel>()
            };
            foreach (var message in messages)
            {
                _sut.AddMessage(message);
            }

            var result = _sut.SearchUserMessages(userId, contactId, "Love");

            result.Should().HaveCount(4);
        }
    }
}