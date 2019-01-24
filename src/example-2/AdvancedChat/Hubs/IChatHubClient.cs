using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedChat.Dto;

namespace AdvancedChat.Hubs
{
    public interface IChatHubClient
    {
        Task ReceiveMessage(MessageDto message);
    }
}
