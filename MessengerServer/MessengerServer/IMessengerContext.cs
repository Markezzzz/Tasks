using MessengerServer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerServer
{
    public interface IMessengerContext
    {
        DbSet<UserDataModel> UserDataModels { get; set; }
        DbSet<ContactDataModel> ContactDataModels { get; set; }
        DbSet<MessageDataModel> MessageDataModels { get; set; }

        int SaveChanges();
    }
}