using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Telegrator.Configuration;
using Telegrator.Hosting.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Providers;

namespace Telegrator.Hosting.Providers
{
    public class HostHandlersCollection(IServiceCollection hostServiceColletion, IHandlersCollectingOptions options) : HandlersCollection(options)
    {
        private readonly IServiceCollection Services = hostServiceColletion;
        public readonly List<Action<TelegramBotHostBuilder>> PreBuilderRoutines = [];
        protected override bool MustHaveParameterlessCtor => false;

        public override IHandlersCollection AddHandler(Type handlerType)
        {
            //
            if (handlerType.GetInterface(nameof(IPreBuildingRoutine)) != null)
            {
                MethodInfo? methodInfo = handlerType.GetMethod(nameof(IPreBuildingRoutine.PreBuildingRoutine), BindingFlags.Static | BindingFlags.Public);
                if (methodInfo != null)
                {
                    Action<TelegramBotHostBuilder> routineDelegate = methodInfo.CreateDelegate<Action<TelegramBotHostBuilder>>(null);
                    PreBuilderRoutines.Add(routineDelegate);
                }
            }

            return base.AddHandler(handlerType);
        }

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
