using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegrator.Polling;

namespace Telegrator.Hosting.Polling
{
    /// <inheritdoc/>
    public class HostUpdateHandlersPool(IOptions<TelegratorOptions> options, ILogger<HostUpdateHandlersPool> logger) : UpdateHandlersPool(options.Value, options.Value.GlobalCancellationToken)
    {
        private readonly ILogger<HostUpdateHandlersPool> _logger = logger;
    }
}
