using System;

namespace MessengerServer.Domain.Models
{
    public class ContactDataModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ContactId { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}