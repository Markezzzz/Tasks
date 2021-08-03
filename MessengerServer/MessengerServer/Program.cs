using MessengerServer.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MessengerServer
{
    class Program
    {
        static void Main()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MessengerContext>();
            var options = optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=messengerserverdb;Trusted_Connection=True;")
                .Options;
            var context = new MessengerContext(options);
        }
    }
}