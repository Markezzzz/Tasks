using System;
using System.Collections.Generic;
using System.Linq;
using MessengerServer.Domain.Models;
using MessengerServer.Domain.Repositories;

namespace MessengerServer.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMessengerContext _context;

        public UserRepository(IMessengerContext context)
        {
            _context = context;
        }

        public UserDataModel GetUserById(int userId)
        {
            return _context.UserDataModels.Find(userId);
        }

        public UserDataModel GetUserByName(string username)
        {
            return _context.UserDataModels.FirstOrDefault(u => u.Name == username);
        }

        public IEnumerable<UserDataModel> SearchUserByName(string query)
        {
            return _context.UserDataModels.Where(u => u.Name.ToLower().Contains(query.ToLower()));
        }

        public void AddUser(UserDataModel user)
        {
            ValidateUser(user);
            _context.UserDataModels.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(UserDataModel updatedUser)
        {
            ValidateUser(updatedUser);
            var user = _context.UserDataModels.Find(updatedUser.UserId);
            if (user is null) throw new ArgumentException("User is not found");
            user.Name = updatedUser.Name;
            user.Password = updatedUser.Password;
            _context.SaveChanges();
        }

        public void UpdateUserState(int userId, UserState state)
        {
            var user = _context.UserDataModels.Find(userId);
            if (user is null) throw new ArgumentException("User is not found");
            user.State = state;
            _context.SaveChanges();
        }

        private void ValidateUser(UserDataModel user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                throw new ArgumentException("Username is empty");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentException("Password is empty");
            }

            if (_context.UserDataModels.Any(u => u.Name == user.Name))
            {
                throw new ArgumentException("Username already exists");
            }
        }
    }
}