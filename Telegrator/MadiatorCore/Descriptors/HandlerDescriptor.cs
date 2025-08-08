using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Specifies the type of handler descriptor.
    /// </summary>
    public enum DescriptorType
    {
        /// <summary>
        /// General handler descriptor.
        /// </summary>
        General,

        /// <summary>
        /// Keyed handler descriptor (uses a service key).
        /// </summary>
        Keyed,
        
        /// <summary>
        /// Implicit handler descriptor.
        /// </summary>
        Implicit,
        
        /// <summary>
        /// Singleton handler descriptor (single instance).
        /// </summary>
        Singleton
    }

    /// <summary>
    /// Describes a handler, its type, filters, and instantiation logic.
    /// </summary>
    public class HandlerDescriptor
    {
        /// <summary>
        /// The type of the descriptor.
        /// </summary>
        public DescriptorType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the handler.
        /// </summary>
        public Type HandlerType
        {
            get;
            private set;
        }

        /// <summary>
        /// The update type handled by this handler.
        /// </summary>
        public UpdateType UpdateType
        {
            get;
            protected set;
        }

        /// <summary>
        /// The indexer for handler concurrency and priority.
        /// </summary>
        public DescriptorIndexer Indexer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to form a fallback report for debugging purposes.
        /// </summary>
        public bool FormReport
        {
            get;
            set;
        }

        /// <summary>
        /// The set of filters associated with this handler.
        /// </summary>
        public DescriptorFiltersSet? Filters
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the aspects configuration for this handler.
        /// Contains pre and post-execution processors if the handler uses the aspect system.
        /// </summary>
        public DescriptorAspectsSet? Aspects
        {
            get;
            protected set;
        }

        /// <summary>
        /// The service key for keyed handlers.
        /// </summary>
        public object? ServiceKey
        {
            get;
            protected set;
        }

        /// <summary>
        /// Factory for creating handler instances.
        /// </summary>
        public Func<UpdateHandlerBase>? InstanceFactory
        {
            get;
            protected set;
        }

        /// <summary>
        /// Singleton instance of the handler, if applicable.
        /// </summary>
        public UpdateHandlerBase? SingletonInstance
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the display string for this handler descriptor.
        /// Used for debugging and logging purposes.
        /// </summary>
        public string? DisplayString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the lazy initialization action for this handler.
        /// Called when the handler instance needs to be initialized.
        /// </summary>
        public Action<UpdateHandlerBase>? LazyInitialization
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class.
        /// </summary>
        /// <param name="descriptorType">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="dontInspect">Whether to skip inspection of the handler type.</param>
        public HandlerDescriptor(DescriptorType descriptorType, Type handlerType, bool dontInspect = false)
        {
            Type = descriptorType;
            HandlerType = handlerType;
            Indexer = new DescriptorIndexer(0, 0, 0);

            if (!dontInspect)
            {
                UpdateHandlerAttributeBase? pollingHandlerAttribute = HandlerInspector.GetPollingHandlerAttribute(handlerType);
                if (pollingHandlerAttribute != null)
                {
                    UpdateType = pollingHandlerAttribute.Type;
                    Indexer = pollingHandlerAttribute.GetIndexer();
                }

                IFilter<Update>[]? filters = HandlerInspector.GetFilterAttributes(handlerType, UpdateType).ToArray();
                IFilter<Update>? stateKeepFilter = HandlerInspector.GetStateKeeperAttribute(handlerType);
                DescriptorAspectsSet? aspects = HandlerInspector.GetAspects(handlerType);

                if (filters.Length > 0 || stateKeepFilter != null)
                {
                    Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter);
                }

                if (aspects != null)
                {
                    Aspects = aspects;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class for keyed handlers.
        /// </summary>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="serviceKey">The service key for the handler.</param>
        public HandlerDescriptor(Type handlerType, object serviceKey) : this(DescriptorType.Keyed, handlerType)
        {
            ServiceKey = serviceKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with complete configuration.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="filters">The set of filters associated with this handler.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class for singleton handlers.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="filters">The set of filters associated with this handler.</param>
        /// <param name="serviceKey">The service key for the handler.</param>
        /// <param name="singletonInstance">The singleton instance of the handler.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            ServiceKey = serviceKey;
            SingletonInstance = singletonInstance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with instance factory.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="filters">The set of filters associated with this handler.</param>
        /// <param name="instanceFactory">The factory for creating handler instances.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            InstanceFactory = instanceFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with service key and instance factory.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="filters">The set of filters associated with this handler.</param>
        /// <param name="serviceKey">The service key for the handler.</param>
        /// <param name="instanceFactory">The factory for creating handler instances.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            ServiceKey = serviceKey;
            InstanceFactory = instanceFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with polling handler attribute.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            FormReport = pollingHandlerAttribute.FormReport;

            if (filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class for singleton handlers with polling handler attribute.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        /// <param name="serviceKey">The service key for the handler.</param>
        /// <param name="singletonInstance">The singleton instance of the handler.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            FormReport = pollingHandlerAttribute.FormReport;
            ServiceKey = serviceKey;
            SingletonInstance = singletonInstance;

            if (filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with instance factory and polling handler attribute.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        /// <param name="instanceFactory">The factory for creating handler instances.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            FormReport = pollingHandlerAttribute.FormReport;
            InstanceFactory = instanceFactory;

            if (filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with service key, instance factory and polling handler attribute.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        /// <param name="serviceKey">The service key for the handler.</param>
        /// <param name="instanceFactory">The factory for creating handler instances.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            FormReport = pollingHandlerAttribute.FormReport;
            ServiceKey = serviceKey;
            InstanceFactory = instanceFactory;

            if (filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with complete configuration.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="validateFilter">The validation filter.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;

            if (validateFilter != null || filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter, validateFilter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class for singleton handlers with complete configuration.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="validateFilter">The validation filter.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        /// <param name="serviceKey">The service key for the handler.</param>
        /// <param name="singletonInstance">The singleton instance of the handler.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            ServiceKey = serviceKey;
            SingletonInstance = singletonInstance;

            if (validateFilter != null || filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter, validateFilter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with instance factory and complete configuration.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="validateFilter">The validation filter.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        /// <param name="instanceFactory">The factory for creating handler instances.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            InstanceFactory = instanceFactory;

            if (validateFilter != null || filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter, validateFilter);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with service key, instance factory and complete configuration.
        /// </summary>
        /// <param name="type">The type of the descriptor.</param>
        /// <param name="handlerType">The type of the handler.</param>
        /// <param name="updateType">The update type handled by this handler.</param>
        /// <param name="indexer">The indexer for handler concurrency and priority.</param>
        /// <param name="validateFilter">The validation filter.</param>
        /// <param name="filters">The array of filters associated with this handler.</param>
        /// <param name="stateKeepFilter">The state keeper filter.</param>
        /// <param name="serviceKey">The service key for the handler.</param>
        /// <param name="instanceFactory">The factory for creating handler instances.</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            ServiceKey = serviceKey;
            InstanceFactory = instanceFactory;

            if (validateFilter != null || filters != null || stateKeepFilter != null)
            {
                Filters = new DescriptorFiltersSet(filters ?? [], stateKeepFilter, validateFilter);
            }
        }

        /// <summary>
        /// Sets the singleton instance for this handler descriptor.
        /// </summary>
        /// <param name="instance">The singleton instance to set.</param>
        public void SetInstance(UpdateHandlerBase instance)
        {
            if (Type != DescriptorType.Singleton)
                throw new InvalidOperationException("Cannot set instance for non-singleton descriptor");

            SingletonInstance = instance;
        }

        /// <summary>
        /// Attempts to set the singleton instance for this handler descriptor.
        /// </summary>
        /// <param name="instance">The singleton instance to set.</param>
        /// <returns>True if the instance was set successfully; otherwise, false.</returns>
        public bool TrySetInstance(UpdateHandlerBase instance)
        {
            if (Type != DescriptorType.Singleton)
                return false;

            SingletonInstance = instance;
            return true;
        }

        /// <summary>
        /// Returns a string representation of this handler descriptor.
        /// </summary>
        /// <returns>A string representation of the handler descriptor.</returns>
        public override string ToString()
        {
            return DisplayString ?? $"{Type} {HandlerType.Name}";
        }
    }
}
