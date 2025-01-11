using Microsoft.AspNetCore.SignalR;

namespace LocationApp.Account.Hubs
{
    public class SearchHub : Hub
    {
        public async Task NotifyNewSearch(string searchQuery)
        {
            await Clients.All.SendAsync("ReceiveSearchNotification", searchQuery);
        }
    }
}
