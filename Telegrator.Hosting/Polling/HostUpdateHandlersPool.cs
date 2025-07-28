using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegrator.Configuration;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Polling;

namespace Telegrator.Hosting.Polling
{
    /// <inheritdoc/>
    public class HostUpdateHandlersPool(IOptions<TelegramBotOptions> options, ILogger<HostUpdateHandlersPool> logger) : UpdateHandlersPool(options.Value, options.Value.GlobalCancellationToken)
    {
        private readonly ILogger<HostUpdateHandlersPool> _logger = logger;

        /// <inheritdoc/>
        protected override async Task ExecuteHandlerWrapper(DescribedHandlerInfo enqueuedHandler)
        {
            //_logger.LogInformation("Handler \"{0}\" has entered execution pool", enqueuedHandler.DisplayString);
            await base.ExecuteHandlerWrapper(enqueuedHandler);
        }
    }
}
