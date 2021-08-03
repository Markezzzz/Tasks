using System.Collections.Generic;
using MessengerServer.Domain.Models;

namespace MessengerServer.Domain.Repositories
{
    public interface IUserRepository
    {
        UserDataModel GetUserById(int userId);
        UserDataModel GetUserByName(string username);
        IEnumerable<UserDataModel> SearchUserByName(string query);
        void AddUser(UserDataModel user);
        void UpdateUser(UserDataModel updatedUser);
        void UpdateUserState(int userId, UserState state);
    }
}