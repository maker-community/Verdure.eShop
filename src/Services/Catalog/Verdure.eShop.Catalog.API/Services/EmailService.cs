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

        public Task SendCatalogItemToAuditAsync(SendEmailToManagerIntegrationEvent eventData)
        {
            _logger.LogInformation("Sending order confirmation email for order {OrderId} to {BuyerEmail}.",
                eventData.CatalogItemId, Email);

            var message = CreateEmailBody(eventData);

            return _daprClient.InvokeBindingAsync(
                SendMailBinding,
                CreateBindingOperation,
                message,
                new Dictionary<string, string>
                {
                    ["emailFrom"] = "gil.zhang.dev@foxmail.com",
                    ["emailTo"] = Email,
                    ["subject"] = $"表情【{eventData.CatalogItemName}】的审核邮件"
                });
        }

        private static string CreateEmailBody(SendEmailToManagerIntegrationEvent eventData) =>
            $@"
            <html>
            <body>
                <h1>有需要审核的表情文件</h1>
                <img src=""http://api.douwp.club/api/Pics/{eventData.PictureFileName}"" height=""100"" width=""100"" alt=""表情图片"">
                <p>表情名称为： {eventData.CatalogItemName}</p>
                <p>审核通过Token：{eventData.AuditToken}</p>
                <ol>
                	<li>请谨慎操作</li>
                    <li>仔细检查内容是否违规</li>
                    <li>使用审核通过Token操作表情状态</li>
                </ol>
                <p>来自创客社区（Hacker space）的问候</p>
            </body>
            </html>";
    }
}
