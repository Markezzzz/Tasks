using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using MessengerServer.Data.Repositories;
using MessengerServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MessengerServer.UnitTests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly UserRepository _sut;
        private readonly MessengerContext _context;
        private readonly Fixture _fixture;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<MessengerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _fixture = new Fixture();
            _context = new MessengerContext(options);
            _sut = new UserRepository(_context);
        }

        [Fact]
        public void AddUser_ShouldAddUser_WhenUserIsValid()
        {
            var user = _fixture.Create<UserDataModel>();

            _sut.AddUser(user);

            _context.UserDataModels.Should().Contain(u =>
                u.Name == user.Name &&
                u.Password == user.Password &&
                u.State == user.State);
        }

        [Fact]
        public void AddUser_ShouldFailValidation_WhenUserNameIsInvalid()
        {
            var user = _fixture.Build<UserDataModel>()
                .Without(u => u.Name)
                .Create();

            Action act = () => _sut.AddUser(user);

            act.Should().Throw<ArgumentException>().WithMessage("Username is empty");
        }

        [Fact]
        public void AddUser_ShouldFailValidation_WhenPasswordIsInvalid()
        {
            var user = _fixture.Build<UserDataModel>()
                .Without(u => u.Password)
                .Create();

            Action act = () => _sut.AddUser(user);

            act.Should().Throw<ArgumentException>().WithMessage("Password is empty");
        }

        [Fact]
        public void AddUser_ShouldFailValidation_WhenUserNameAlreadyExists()
        {
            var user1 = _fixture.Build<UserDataModel>()
                .With(u => u.Name, "Mark")
                .Create();
            var user2 = _fixture.Build<UserDataModel>()
                .With(u => u.Name, "Mark")
                .Create();
            _sut.AddUser(user1);

            Action act = () => _sut.AddUser(user2);

            act.Should().Throw<ArgumentException>().WithMessage("Username already exists");
        }

        [Fact]
        public void UpdateUser_ShouldUpdateUser_WhenUpdatedUserIsValid()
        {
            var user = _fixture.Create<UserDataModel>();
            var updatedUser = _fixture.Build<UserDataModel>()
                .With(u => u.UserId, user.UserId)
                .Create();
            _sut.AddUser(user);

            _sut.UpdateUser(updatedUser);

            _context.UserDataModels.Should().Contain(u =>
                u.Name == updatedUser.Name &&
                u.Password == updatedUser.Password);
        }

        [Fact]
        public void UpdateUser_ShouldFail_WhenUserDoesNotExist()
        {
            var user = _fixture.Create<UserDataModel>();

            Action act = () => _sut.UpdateUser(user);

            act.Should().Throw<ArgumentException>().WithMessage("User is not found");
        }

        [Fact]
        public void UpdateUserState_ShouldUpdateState()
        {
            var user = _fixture.Build<UserDataModel>()
                .With(u => u.State, UserState.Offline)
                .Create();
            _sut.AddUser(user);

            _sut.UpdateUserState(user.UserId, UserState.Online);

            _context.UserDataModels.Should().Contain(u =>
                u.Name == user.Name &&
                u.Password == user.Password &&
                u.State == UserState.Online);
        }

        [Fact]
        public void UpdateUserState_ShouldFail_WhenUserDoesNotExist()
        {
            var user = _fixture.Create<UserDataModel>();

            Action act = () => _sut.UpdateUserState(user.UserId, UserState.Online);

            act.Should().Throw<ArgumentException>().WithMessage("User is not found");
        }

        [Fact]
        public void GetUserById_ShouldReturnUser_WhenUserExists()
        {
            var user = _fixture.Create<UserDataModel>();
            _sut.AddUser(user);

            var result = _sut.GetUserById(user.UserId);

            result.Should().Be(user);
        }

        [Fact]
        public void GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var user = _fixture.Create<UserDataModel>();

            var result = _sut.GetUserById(user.UserId);

            result.Should().BeNull();
        }

        [Fact]
        public void GetUserByName_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var user = _fixture.Create<UserDataModel>();

            var result = _sut.GetUserByName(user.Name);

            result.Should().BeNull();
        }

        [Fact]
        public void GetUserByName_ShouldReturnUser_WhenUserExists()
        {
            var user = _fixture.Create<UserDataModel>();
            _sut.AddUser(user);

            var result = _sut.GetUserByName(user.Name);

            result.Should().Be(user);
        }

        [Fact]
        public void SearchUserByName_ShouldReturnUser_WhenUsersExist()
        {
            var users = new List<UserDataModel>()
            {
                _fixture.Create<UserDataModel>(),
                _fixture.Create<UserDataModel>(),
                _fixture.Create<UserDataModel>(),
                _fixture.Build<UserDataModel>().With(u => u.Name, "LoveCats").Create(),
                _fixture.Build<UserDataModel>().With(u => u.Name, "LoveDogs").Create(),
                _fixture.Build<UserDataModel>().With(u => u.Name, "LoveAnimals").Create(),
                _fixture.Build<UserDataModel>().With(u => u.Name, "LoveEverything").Create(),
                _fixture.Create<UserDataModel>()
            };
            foreach (var user in users)
                _sut.AddUser(user);

            var result = _sut.SearchUserByName("love");

            result.Should().HaveCountGreaterOrEqualTo(4);
        }

        [Fact]
        public void SearchUserByName_ShouldBeEmpty_WhenUsersNotFound()
        {
            var result = _sut.SearchUserByName("AgentSmith");

            result.Should().BeEmpty();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}