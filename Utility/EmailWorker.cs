using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class EmailWorker : BackgroundService
{
    private readonly ILogger<EmailWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEmailSender _emailSender;

    public EmailWorker(ILogger<EmailWorker> logger, IServiceScopeFactory scopeFactory, IEmailSender emailSender)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _emailSender = emailSender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    try
                    {
                        // Get all orders with null ShippedDate
                        var orders = unitOfWork.Order.GetAll(o => o.ShippedDate == null);

                        // Group orders by AccountId
                        var ordersGroupedByAccount = orders.GroupBy(o => o.AccountId)
                                                           .ToDictionary(g => g.Key, g => g.AsEnumerable());
                        foreach (var item in ordersGroupedByAccount)
                        {
                            // Send email to request user confirmation
                            await SendConfirmationEmail(item, unitOfWork);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing emails.");
                    }

                    //Send email ads to user
                   await SendAdsEmail(unitOfWork);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Wait for 24 hours before checking again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing emails.");
            }
        }

        _logger.LogInformation("Email Worker stopped.");
    }

    private async Task SendConfirmationEmail(KeyValuePair<string, IEnumerable<Order>> entry, IUnitOfWork unitOfWork)
    {
        // Assuming you have a method to get the base URL dynamically, e.g., from configuration or request context
        var baseUrl = "https://localhost:7187"; // Replace with your method to get the base URL dynamically

        // Construct the confirmation link
        var confirmationLink = $"{baseUrl}/OrderHistory";

        var accountId = entry.Key;
        var accountOrders = entry.Value;

        var userEmail = unitOfWork.Account.GetFirstOrDefault(c => c.Id == accountId)?.Email;

        if (!string.IsNullOrEmpty(userEmail))
        {
            // Construct the email body
            var emailBody = "<p>Dear Customer,</p>" +
                            "<p>We noticed that the following orders have not yet been marked as shipped:</p><ul>";

            foreach (var order in accountOrders)
            {
                emailBody += $"<li>Order ID: {order.OrderID}</li>";
            }

            emailBody += "</ul>" +
                         "<p>To help us process your orders promptly and ensure timely delivery, we kindly request you to confirm whether " +
                         "the shipping details are correct or if there are any changes needed.</p>" +
                         $"<p>Please click on the link below to review and confirm your order details:</p>" +
                         $"<p><a href='{confirmationLink}'>Review and Confirm Orders</a></p>" +
                         "<p>Thank you for choosing us for your shopping needs! We look forward to serving you soon.</p>" +
                         "<p>Best regards,<br>Customer Service Team</p>";

            // Send the email with all order IDs for the current AccountId
            await _emailSender.SendEmailAsync(
                userEmail,
                "Important: Update on Your Orders Status",
                emailBody
            );

            _logger.LogInformation($"Confirmation email sent to {userEmail} with all order IDs.");
        }
        else
        {
            _logger.LogWarning($"No email found for account ID {accountId}.");
        }
    }


    private async Task SendAdsEmail(IUnitOfWork unitOfWork)
    {
        // Get user emails
        var userEmail = unitOfWork.Account.GetAll().Select(a => a.Email);
        var adses = unitOfWork.Ads.GetAll().Where(a => a.StartDate <= DateTime.Now && a.EndDate >= DateTime.Now);
        if(adses != null)
        {
            foreach(var ads in adses)
            {
                var emailbody = "<p>Dear Customer,</p>" +
                                "<p>We have a special promotion for you:</p>";
                emailbody += $"<p>{ads.Content}</p>";
                emailbody += "<p>Thank you for choosing us for your shopping needs! We look forward to serving you soon.</p>" +
                             "<p>Best regards,<br>Customer Service Team</p>";

                foreach (var email in userEmail)
                {
                    // Send the email with all order IDs for the current AccountId
                    await _emailSender.SendEmailAsync(
                        email,
                        ads.Title,
                        emailbody
                    );
                }
            }
        }
    }
}
