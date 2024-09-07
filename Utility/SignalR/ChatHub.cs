using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

public class ChatHub : Hub
{
    // List of available admins (staff)
    private static ConcurrentBag<string> availableAdmins = new ConcurrentBag<string>();

    // Mapping customers (NormalUser) to their assigned admin
    private static ConcurrentDictionary<string, string> customerAdminMapping = new ConcurrentDictionary<string, string>();

    // Method when a customer connects to the chat
    public async Task CustomerConnect()
    {
        // If there are available admins, assign the customer to one
        if (availableAdmins.Count > 0)
        {
            string assignedAdmin = availableAdmins.FirstOrDefault();
            if (assignedAdmin != null)
            {
                string connectionId = Context.ConnectionId;
                customerAdminMapping.TryAdd(connectionId, assignedAdmin);
                await Clients.Client(assignedAdmin).SendAsync("NewCustomerAssigned", assignedAdmin, connectionId);
                await Clients.Caller.SendAsync("AdminAssigned", assignedAdmin, connectionId);
            }
        }
        else
        {
            await Clients.Caller.SendAsync("NoAdminAvailable");
        }
    }

    // When an admin  (staff) or NormalUser connects to the hub
    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        // Accessing user claims from the context
        var user = Context.User;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Staff")
        {
            availableAdmins.Add(connectionId);
        }
        await base.OnConnectedAsync();
    }

    // Handle when a user (customer or admin) disconnects
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;
        availableAdmins = new ConcurrentBag<string>(availableAdmins.Except(new[] { connectionId }));

        var customerPair = customerAdminMapping.FirstOrDefault(x => x.Value == connectionId);
        if (customerPair.Key != null)
        {
            customerAdminMapping.TryRemove(customerPair.Key, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Send message from customer to admin
    public async Task SendMessageToAdmin(string customerId, string message)
    {
        if (customerAdminMapping.TryGetValue(customerId, out string adminId))
        {
            await Clients.Client(adminId).SendAsync("ReceiveMessageFromCustomer", customerId, message);
        }
    }

    // Send message from admin to customer
    public async Task SendMessageToCustomer(string customerId, string message)
    {
        if (customerAdminMapping.TryGetValue(customerId, out string adminId))
        {
            await Clients.Client(customerId).SendAsync("ReceiveMessageFromAdmin", message);
        }
    }
}
