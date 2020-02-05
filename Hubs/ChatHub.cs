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
            int idReceiver = 0;

            //get the id of manager
            if (receiverRole.Contains("Manager"))
            {
                idReceiver = 4;
            }
            
            Clients.All.receiveNotification(idReceiver);//

        }

    }
}