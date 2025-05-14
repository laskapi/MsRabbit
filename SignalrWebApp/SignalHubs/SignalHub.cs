using Microsoft.AspNetCore.SignalR;

namespace SignaRWebApp.Hubs
{
    public class SignalHub:Hub
    {
        public async Task SendMEssage(string message)
        {
            await Clients.All.SendAsync("Received", message);
        }
    }
}
