using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Team8ADProjectSSIS.DAO;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.Hubs
{
    public class ChatHub : Hub
    {
        
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Announce(string message)
        {
            Clients.All.announce(message);
        }
        public void Send(string name, string message)
        {  
            Clients.All.addNewMessageToPage(name, message);
        }

        public void SendNotificationByGroupByRole(int idSender, string receiverRole, string message)
        {

            //get the id of manager
            if (receiverRole.Contains("Manager"))
            {
                //4 is manager
                Clients.All.receiveNotification(4);//
            }

            else if (receiverRole.Contains("Clerk"))
            {
                //1,2,3 are clerks
                Clients.All.receiveNotification(1);//
                Clients.All.receiveNotification(2);//
                Clients.All.receiveNotification(3);//
            }else if (receiverRole.Contains("Supervisor"))
            {
                //5 is manager
                Clients.All.receiveNotification(5);//
            }
            else
            {

            }

        }

        public void SendNotificationById(int idReceiver)
        {
            Clients.All.receiveNotification(idReceiver);//
        }


    }
}