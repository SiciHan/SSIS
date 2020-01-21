using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Team8ADProjectSSIS.Hubs
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        // Develop a hub as the main coordination object on the server, 
        //and using the SignalR jQuery library to send and receive messages.
        public void Send(string name, string message)
        {
            //Call this method to update clients
            Clients.All.addNewMessageToPage(name, message);
        }
/*        Clients call the method to send a new message.
 *        ChatHub.Send
 *        The hub in turn sends the name and message to all clients by calling.
 *        Clients.All.addNewMessageToPage
 *        To Call a function on the client to update the clients.
 *        Clients.All.addNewMessageToPage(name, message);
 *        Use this property to access all clients connected to this hub.
 *        Microsoft.AspNet.SignalR.Hub.Clients*/
    }
}