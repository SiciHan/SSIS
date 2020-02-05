using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public List<NotificationChannel> FindAllNotifications()
        {
            return context.NotificationChannels.OfType<NotificationChannel>().Include(x => x.Notification).ToList();
        }

        public List<NotificationChannel> FindAllNotificationsByIdReceiver(int idReceiver)
        {
            List<NotificationChannel> nclist=context.NotificationChannels.OfType<NotificationChannel>().Where(x=>x.IdTo==idReceiver).Include(x => x.Notification).Include(x=>x.From).ToList();
/*            foreach(NotificationChannel nc in nclist)
            {
                nc.IsRead = true;
            }*/
            context.SaveChanges();
            return nclist;
        }

        public void CreateNotificationsToGroup(string role,int IdSender,string message)
        {
            Notification noti = new Notification();
            noti.Text = message;
            context.Notifications.Add(noti);
            context.SaveChanges();

            //find list of recievers by role
            List<Employee> receivers = context.Employees.OfType<Employee>().Where(x => x.Role.Label.Equals(role)).ToList();

            foreach(Employee e in receivers)
            {
                NotificationChannel nc = new NotificationChannel();
                nc.IdNotification = noti.IdNotification;
                nc.IdFrom = IdSender;
                nc.IdTo = e.IdEmployee;
                nc.Date = DateTime.Now;
                nc.IsRead = false;
                context.NotificationChannels.Add(nc);
                context.SaveChanges();
            }
        }

        public NotificationChannel MarkAsUnreadById(int idNC)
        {
            NotificationChannel nc=context.NotificationChannels.OfType<NotificationChannel>().Where(x => x.IdNC == idNC).FirstOrDefault();
            nc.IsRead = false;
            context.SaveChanges();
            return nc;

        }

        public NotificationChannel MarkAsReadById(int idNC)
        {
            NotificationChannel nc = context.NotificationChannels.OfType<NotificationChannel>().Where(x => x.IdNC == idNC).FirstOrDefault();
            nc.IsRead = true;
            context.SaveChanges();
            return nc;
        }
/*        private readonly SSISContext context;

        public NotificationChannelDAO()
        {
            this.context = new SSISContext();
        }*/

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

        internal int GetUnreadNotificationCount(int idReceiver)
        {
            List<NotificationChannel> ncs = context.NotificationChannels.OfType<NotificationChannel>().Where(x => x.IdTo == idReceiver && x.IsRead == false).ToList();
            return ncs.Count;
        }
    }
}