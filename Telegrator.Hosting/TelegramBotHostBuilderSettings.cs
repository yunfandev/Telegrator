using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Telegrator.Hosting
{
    /// <summary>
    /// Settings os hosted Telegram bot
    /// </summary>
    public class TelegramBotHostBuilderSettings() : TelegratorOptions
    {
        /// <inheritdoc cref="HostApplicationBuilderSettings.DisableDefaults"/>
        public bool DisableDefaults { get; set; }

        /// <inheritdoc cref="HostApplicationBuilderSettings.Args"/>
        public string[]? Args { get; set; }

        /// <inheritdoc cref="HostApplicationBuilderSettings.Configuration"/>
        public ConfigurationManager? Configuration { get; set; }

        /// <inheritdoc cref="HostApplicationBuilderSettings.EnvironmentName"/>
        public string? EnvironmentName { get; set; }

        /// <inheritdoc cref="HostApplicationBuilderSettings.ApplicationName"/>
        public string? ApplicationName { get; set; }

        /// <inheritdoc cref="HostApplicationBuilderSettings.ContentRootPath"/>
        public string? ContentRootPath { get; set; }

        internal HostApplicationBuilderSettings ToApplicationBuilderSettings() => new HostApplicationBuilderSettings()
        {
            DisableDefaults = DisableDefaults,
            Args = Args,
            Configuration = Configuration,
            EnvironmentName = EnvironmentName,
            ApplicationName = ApplicationName,
            ContentRootPath = ContentRootPath
        };
    }
}
