//@SHutong
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Team8ADProjectSSIS.Models
{
    public class Notification
    {
        [Key]
        public int IdNotification{get; set; }
        public string Text { get; set; }

        public virtual ICollection<NotificationChannel> NotificationChannels { get; set; }
    }
}