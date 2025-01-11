using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // Connection to the first SignalR hub
        var connectionAccount = new HubConnectionBuilder()
            .WithUrl("https://localhost:44387/searchHub")
            .Build();

        connectionAccount.On<string>("ReceiveSearchNotification", (message) =>
        {
            Console.WriteLine($"[Account] New search notification: {message}");
        });

        // Connection to the second SignalR hub
        var connectionLocation = new HubConnectionBuilder()
            .WithUrl("https://localhost:44351/searchHub")
            .Build();

        connectionLocation.On<string>("ReceiveSearchNotification", (message) =>
        {
            Console.WriteLine($"[Location] New search notification: {message}");
        });

        // Start both connections
        await connectionAccount.StartAsync();
        await connectionLocation.StartAsync();

        Console.WriteLine("Connected to both SignalR hubs.");

        Console.ReadLine();
    }
}