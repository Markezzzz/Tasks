using System;
using System.ComponentModel.DataAnnotations;

namespace MessengerServer.Domain.Models
{
    public class MessageDataModel
    {
        [Key] public int MessageId { get; set; }
        public int UserId { get; set; }
        public int ContactId { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string Content { get; set; }
    }
}