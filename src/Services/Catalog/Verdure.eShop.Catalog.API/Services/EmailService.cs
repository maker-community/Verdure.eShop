namespace Verdure.eShop.Services.Catalog.API.Services
{
    public class EmailService : IEmailService
    {
        private const string SendMailBinding = "sendmail";
        private const string Email = "gil.zhang.dev@outlook.com";
        private const string CreateBindingOperation = "create";

        private readonly DaprClient _daprClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(DaprClient daprClient, ILogger<EmailService> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public Task SendCatalogItemToAuditAsync(CatalogItem catalog)
        {
            _logger.LogInformation("Sending order confirmation email for order {OrderId} to {BuyerEmail}.",
                catalog.Id, Email);

            var message = CreateEmailBody(catalog);

            return _daprClient.InvokeBindingAsync(
                SendMailBinding,
                CreateBindingOperation,
                message,
                new Dictionary<string, string>
                {
                    ["emailFrom"] = "gil.zhang.dev@foxmail.com",
                    ["emailTo"] = Email,
                    ["subject"] = $"Your eShopOnDapr Order #{Email}"
                });
        }

        private static string CreateEmailBody(CatalogItem catalog) =>
            $@"
            <html>
            <body>
                <h1>Your order confirmation</h1>
                <p>Thank you for your order! The order number is {catalog.Id}.</p>
                <p>To follow the status of your order:</p>
                <ol>
                	<li>Log onto the eShopOnDapr website</li>
                    <li>Hover your mouse cursor over your account icon in the top-right corner</li>
                    <li>Select 'My Orders' in the context-menu that appears</li>
                    <li>Click the 'Details' link for order with number {catalog.Id}</li>
                </ol>
                <p>Greetings,</p>
                <p>The eShopOnDapr Team</p>
            </body>
            </html>";
    }
}
