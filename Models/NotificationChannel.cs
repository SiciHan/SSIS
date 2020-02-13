//@SHutong
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team8ADProjectSSIS.Models
{
    public class NotificationChannel
    {
        [Key]
        public int IdNC { get; set; }

        public int IdFrom { get; set; }
        [ForeignKey("IdFrom")]
        public Employee From { get; set; }
        public int IdTo { get; set; }
        [ForeignKey("IdTo")]
        public Employee To { get; set; }
        public int IdNotification { get; set; }
        [ForeignKey("IdNotification")]
        public Notification Notification { get; set; }
        public bool IsRead { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime Date { get; set; }

    }
}