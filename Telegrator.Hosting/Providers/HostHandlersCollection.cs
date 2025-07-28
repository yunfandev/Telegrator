using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Telegrator.Configuration;
using Telegrator.Hosting.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Providers;

namespace Telegrator.Hosting.Providers
{
    /// <summary>
    /// Pre host building task
    /// </summary>
    /// <param name="builder"></param>
    public delegate void PreBuildingRoutine(ITelegramBotHostBuilder builder);

    /// <inheritdoc/>
    public class HostHandlersCollection(IServiceCollection hostServiceColletion, ITelegratorOptions options) : HandlersCollection(options)
    {
        private readonly IServiceCollection Services = hostServiceColletion;

        /// <inheritdoc/>
        protected override bool MustHaveParameterlessCtor => false;

        /// <summary>
        /// List of tasks that should be completed right before building the bot
        /// </summary>
        public readonly List<PreBuildingRoutine> PreBuilderRoutines = [];

        /// <inheritdoc/>
        public override IHandlersCollection AddHandler(Type handlerType)
        {
            if (handlerType.IsPreBuildingRoutine(out MethodInfo? routineMethod))
                PreBuilderRoutines.Add(routineMethod.CreateDelegate<PreBuildingRoutine>(null));

            return base.AddHandler(handlerType);
        }

        /// <inheritdoc/>
        public override IHandlersCollection AddDescriptor(HandlerDescriptor descriptor)
        {
            switch (descriptor.Type)
            {
                case DescriptorType.General:
                    {
                        if (descriptor.InstanceFactory != null)
                            Services.AddScoped(descriptor.HandlerType, _ => descriptor.InstanceFactory.Invoke());
                        else
                            Services.AddScoped(descriptor.HandlerType);

                        break;
                    }

                case DescriptorType.Keyed:
                    {
                        if (descriptor.InstanceFactory != null)
                            Services.AddKeyedScoped(descriptor.HandlerType, descriptor.ServiceKey, (_, _) => descriptor.InstanceFactory.Invoke());
                        else
                            Services.AddKeyedScoped(descriptor.HandlerType, descriptor.ServiceKey);

                        break;
                    }

                case DescriptorType.Singleton:
                    {
                        Services.AddSingleton(descriptor.HandlerType, descriptor.SingletonInstance ?? (descriptor.InstanceFactory != null
                            ? descriptor.InstanceFactory.Invoke()
                            : throw new Exception()));

                        break;
                    }

                case DescriptorType.Implicit:
                    {
                        Services.AddKeyedSingleton(descriptor.HandlerType, descriptor.ServiceKey, descriptor.SingletonInstance ?? (descriptor.InstanceFactory != null
                            ? descriptor.InstanceFactory.Invoke()
                            : throw new Exception()));
                        
                        break;
                    }
            }

            return base.AddDescriptor(descriptor);
        }
    }
}
