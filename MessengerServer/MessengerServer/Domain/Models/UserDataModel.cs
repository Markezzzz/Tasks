using System.ComponentModel.DataAnnotations;

namespace MessengerServer.Domain.Models
{
    public class UserDataModel
    {
        [Key] public int UserId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public UserState State { get; set; }
    }

    public enum UserState
    {
        Offline,
        Online
    }
}