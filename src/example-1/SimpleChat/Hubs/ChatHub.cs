using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Hubs
{
    public class ChatHub : Hub
    {
        public void SendMessage(string name, string message)
        {
            Clients.All.SendAsync("ReceiveMessage", name, message);
        }
    }
}
