using MessengerServer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerServer
{
    public class MessengerContext : DbContext, IMessengerContext
    {
        public virtual DbSet<UserDataModel> UserDataModels { get; set; }
        public virtual DbSet<ContactDataModel> ContactDataModels { get; set; }
        public virtual DbSet<MessageDataModel> MessageDataModels { get; set; }

        public MessengerContext(DbContextOptions<MessengerContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public MessengerContext()
        {
        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=messengerserverdb;Trusted_Connection=True;");
        // }
    }
}