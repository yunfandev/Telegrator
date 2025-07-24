using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegrator.Configuration;

namespace Telegrator.Hosting
{
    /// <summary>
    /// 
    /// </summary>
    public class TelegramBotHostBuilderSettings() : IHandlersCollectingOptions
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

        /// <inheritdoc/>
        public bool DescendDescriptorIndex { get; set; } = true;

        /// <inheritdoc/>
        public bool ExceptIntersectingCommandAliases { get; set; } = true;
        
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
