using System.Reflection;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations;
using Telegrator.Attributes;
using Telegrator.Configuration;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Providers
{
    /// <summary>
    /// Collection class for managing handler descriptors organized by update type.
    /// Provides functionality for collecting, adding, and organizing handlers.
    /// </summary>
    /// <param name="options">Optional configuration options for handler collecting.</param>
    public class HandlersCollection(ITelegratorOptions? options) : IHandlersCollection
    {
        private readonly List<UpdateType> _allowedTypes = [];

        /// <summary>
        /// Dictionary that organizes handler descriptors by update type.
        /// </summary>
        protected readonly Dictionary<UpdateType, HandlerDescriptorList> InnerDictionary = [];
        
        /// <summary>
        /// Configuration options for handler collecting.
        /// </summary>
        protected readonly ITelegratorOptions? Options = options;
        
        /// <summary>
        /// Gets whether handlers must have a parameterless constructor.
        /// </summary>
        protected virtual bool MustHaveParameterlessCtor => true;
        
        /// <summary>
        /// List of command aliases that have been registered.
        /// </summary>
        public readonly List<string> CommandAliasses = [];

        /// <inheritdoc/>
        public IEnumerable<UpdateType> AllowedTypes => _allowedTypes;

        /// <inheritdoc/>
        public IEnumerable<UpdateType> Keys
        {
            get => InnerDictionary.Keys;
        }

        /// <inheritdoc/>
        public IEnumerable<HandlerDescriptorList> Values
        {
            get => InnerDictionary.Values;
        }

        /// <inheritdoc/>
        public HandlerDescriptorList this[UpdateType updateType]
        {
            get => InnerDictionary[updateType];
        }

        /// <summary>
        /// Adds a handler descriptor to the collection.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to add.</param>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the handler type doesn't have a parameterless constructor and MustHaveParameterlessCtor is true.</exception>
        public virtual IHandlersCollection AddDescriptor(HandlerDescriptor descriptor)
        {
            if (MustHaveParameterlessCtor && !descriptor.HandlerType.HasParameterlessCtor())
                throw new Exception("This handler (" + descriptor.HandlerType.FullName + "), must contain constructor without parameters.");

            _allowedTypes.Union(descriptor.UpdateType);
            MightAwaitAttribute[] mightAwaits = descriptor.HandlerType.GetCustomAttributes<MightAwaitAttribute>().ToArray();
            if (mightAwaits.Length > 0)
                _allowedTypes.Union(mightAwaits.SelectMany(attr => attr.UpdateTypes));

            IntersectCommands(descriptor);
            HandlerDescriptorList list = GetDescriptorList(descriptor);

            if (descriptor.UpdateType == UpdateType.InlineQuery || descriptor.UpdateType == UpdateType.ChosenInlineResult)
            {
                if (list.Count > 0)
                    throw new Exception("Bot cannot have more than one InlineQuery handler");
            }

            list.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Adds a handler type to the collection.
        /// </summary>
        /// <typeparam name="THandler">The type of handler to add.</typeparam>
        /// <returns>This collection instance for method chaining.</returns>
        public virtual IHandlersCollection AddHandler<THandler>() where THandler : UpdateHandlerBase
        {
            AddHandler(typeof(THandler));
            return this;
        }

        /// <summary>
        /// Adds a handler type to the collection.
        /// </summary>
        /// <param name="handlerType">The type of handler to add.</param>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the type is not a valid handler implementation.</exception>
        public virtual IHandlersCollection AddHandler(Type handlerType)
        {
            if (!handlerType.IsHandlerRealization())
                throw new Exception();

            if (handlerType.IsCustomDescriptorsProvider())
            {
                foreach (HandlerDescriptor handlerDescriptor in InvokeCustomDescriptorsProvider(handlerType))
                    AddDescriptor(handlerDescriptor);
            }
            else
            {
                HandlerDescriptor descriptor = new HandlerDescriptor(DescriptorType.General, handlerType);
                AddDescriptor(descriptor);
            }

            return this;
        }

        /// <summary>
        /// Gets or creates a descriptor list for the specified update type.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to get the list for.</param>
        /// <returns>The descriptor list for the update type.</returns>
        public virtual HandlerDescriptorList GetDescriptorList(HandlerDescriptor descriptor)
        {
            UpdateType updateType = UpdateTypeExtensions.SuppressTypes.TryGetValue(descriptor.UpdateType, out UpdateType suppressType) 
                ? suppressType
                : descriptor.UpdateType;

            if (!InnerDictionary.TryGetValue(updateType, out HandlerDescriptorList? list))
            {
                list = new HandlerDescriptorList(updateType, Options);
                InnerDictionary.Add(updateType, list);
            }

            return list;
        }

        /// <summary>
        /// Checks for intersecting command aliases and handles them according to configuration.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to check for command aliases.</param>
        /// <exception cref="Exception">Thrown when intersecting command aliases are found and ExceptIntersectingCommandAliases is enabled.</exception>
        protected void IntersectCommands(HandlerDescriptor descriptor)
        {
            if (Options == null)
                return;

            CommandAlliasAttribute? alliasAttribute = descriptor.HandlerType.GetCustomAttribute<CommandAlliasAttribute>();
            if (alliasAttribute == null)
                return;

            if (Options.ExceptIntersectingCommandAliases && CommandAliasses.Intersect(alliasAttribute.Alliases, StringComparer.InvariantCultureIgnoreCase).Any())
                throw new Exception(descriptor.HandlerType.FullName);

            CommandAliasses.AddRange(alliasAttribute.Alliases);
        }

        /// <summary>
        /// Invokes a custom descriptors provider to get handler descriptors.
        /// </summary>
        /// <param name="handlerType">The handler type that implements ICustomDescriptorsProvider.</param>
        /// <returns>A collection of handler descriptors from the custom provider.</returns>
        /// <exception cref="Exception">Thrown when the handler type doesn't have a parameterless constructor or cannot be instantiated.</exception>
        protected virtual IEnumerable<HandlerDescriptor> InvokeCustomDescriptorsProvider(Type handlerType)
        {
            if (!handlerType.HasParameterlessCtor())
                throw new Exception();

            ICustomDescriptorsProvider? provider = (ICustomDescriptorsProvider?)Activator.CreateInstance(handlerType);
            return provider == null ? throw new Exception() : provider.DescribeHandlers();
        }
    }
}
