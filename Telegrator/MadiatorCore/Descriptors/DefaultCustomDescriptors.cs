using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.Handlers;
using Telegrator.Handlers.Building;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    public class MethodHandlerDescriptor<TUpdate> : HandlerDescriptor where TUpdate : class
    {
        public MethodHandlerDescriptor(AbstractHandlerAction<TUpdate> action) : base(DescriptorType.General)
        {
            UpdateHandlerAttributeBase handlerAttribute = HandlerInspector.GetHandlerAttribute(action.Method);
            StateKeeperAttributeBase? stateKeeperAttribute = HandlerInspector.GetStateKeeperAttribute(action.Method);
            IFilter<Update>[] filters = HandlerInspector.GetFilterAttributes(action.Method, handlerAttribute.Type).ToArray();

            UpdateType = handlerAttribute.Type;
            Indexer = handlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(handlerAttribute, stateKeeperAttribute, filters);
            DisplayString = HandlerInspector.GetDisplayName(action.Method);
            InstanceFactory = () => new MethodHandler(action.Method, UpdateType);
        }

        private class MethodHandler(MethodInfo method, UpdateType updateType) : AbstractUpdateHandler<TUpdate>(updateType)
        {
            private readonly MethodInfo Method = method;

            public override async Task Execute(IAbstractHandlerContainer<TUpdate> container, CancellationToken cancellation)
            {
                if (Method is null)
                    throw new Exception();

                if (Method.ReturnType == typeof(void))
                {
                    Method.Invoke(this, [container, cancellation]);
                    return;
                }
                else
                {
                    object branchReturn = Method.Invoke(this, [container, cancellation]);
                    if (branchReturn == null)
                        return;

                    if (branchReturn is Task branchTask)
                        await branchTask;
                }
            }
        }
    }
}
