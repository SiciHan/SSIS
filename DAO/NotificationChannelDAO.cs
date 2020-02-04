using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class NotificationChannelDAO
    {
        private readonly SSISContext context;

        public NotificationChannelDAO()
        {
            this.context = new SSISContext();
        }

        internal void SendNotification(int IdStoreClerk, int IdEmployee, int notifId, DateTime date)
        {
            NotificationChannel NC = new NotificationChannel()
            {
                IdFrom = IdStoreClerk,
                IdTo = IdEmployee,
                IdNotification = notifId,
                IsRead = false,
                Date = date
            };

            context.NotificationChannels.Add(NC);
            context.SaveChanges();
        }
    }
}