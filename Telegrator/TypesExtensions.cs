using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator.Annotations;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Attributes;
using Telegrator.Filters.Components;
using Telegrator.Handlers;
using Telegrator.Handlers.Building;
using Telegrator.Handlers.Building.Components;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Providers;
using Telegrator.StateKeeping;
using Telegrator.StateKeeping.Components;

namespace Telegrator
{
    /// <summary>
    /// Provides usefull helper methods for messages
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Substrings entity content from text
        /// </summary>
        /// <param name="message"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string SubstringEntity(this Message message, MessageEntity entity)
        {
            if (message.Text == null || string.IsNullOrEmpty(message.Text)) // DO NOT CHANGE! Compiler SOMEHOW warnings "probably null" here
                throw new ArgumentNullException(nameof(message), "Cannot substring entity from message with text that null or empty");

            return message.Text.Substring(entity.Offset, entity.Length);
        }
    }

    /// <summary>
    /// Extension methods for handler containers.
    /// Provides convenient methods for creating awaiter builders and state keeping.
    /// </summary>
    public static class HandlerContainerExtensions
    {
        /// <summary>
        /// Creates an awaiter builder for a specific update type.
        /// </summary>
        /// <typeparam name="TUpdate">The type of update to await.</typeparam>
        /// <param name="container">The handler container.</param>
        /// <param name="updateType">The type of update to await.</param>
        /// <returns>An awaiter builder for the specified update type.</returns>
        public static IAwaiterHandlerBuilder<TUpdate> AwaitUpdate<TUpdate>(this IHandlerContainer container, UpdateType updateType) where TUpdate : class
            => container.AwaitingProvider.CreateAbstract<TUpdate>(updateType, container.HandlingUpdate);

        /// <summary>
        /// Creates an awaiter builder for any update type.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <returns>An awaiter builder for any update type.</returns>
        public static IAwaiterHandlerBuilder<Update> AwaitAny(this IHandlerContainer container)
            => container.AwaitUpdate<Update>(UpdateType.Unknown);

        /// <summary>
        /// Creates an awaiter builder for message updates.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <returns>An awaiter builder for message updates.</returns>
        public static IAwaiterHandlerBuilder<Message> AwaitMessage(this IHandlerContainer container)
            => container.AwaitUpdate<Message>(UpdateType.Message);

        /// <summary>
        /// Creates an awaiter builder for callback query updates.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <returns>An awaiter builder for callback query updates.</returns>
        public static IAwaiterHandlerBuilder<CallbackQuery> AwaitCallbackQuery(this IHandlerContainer container)
            => container.AwaitUpdate<CallbackQuery>(UpdateType.CallbackQuery);

        /// <summary>
        /// Gets a state keeper instance for the specified types.
        /// </summary>
        /// <typeparam name="TKey">The type of the state key.</typeparam>
        /// <typeparam name="TState">The type of the state value.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="_">The handler container (unused).</param>
        /// <returns>The state keeper instance.</returns>
        public static TKeeper GetStateKeeper<TKey, TState, TKeeper>(this IHandlerContainer _) where TKey : notnull where TState : IEquatable<TState> where TKeeper : StateKeeperBase<TKey, TState>, new()
            => StateKeeperAttribute<TKey, TState, TKeeper>.Shared;
    }

    /// <summary>
    /// Provides usefull helper methods for abstract handler containers
    /// </summary>
    public static class AbstractHandlerContainerExtensions
    {
        /// <summary>
        /// Sends a reply message to the current message.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="text">The text of the message to send.</param>
        /// <param name="parseMode">The parse mode for the message text.</param>
        /// <param name="replyMarkup">The reply markup for the message.</param>
        /// <param name="linkPreviewOptions">Options for link preview generation.</param>
        /// <param name="messageThreadId">The thread ID for forum topics.</param>
        /// <param name="entities">The message entities to include.</param>
        /// <param name="disableNotification">Whether to disable notification for the message.</param>
        /// <param name="protectContent">Whether to protect the message content.</param>
        /// <param name="messageEffectId">The message effect ID.</param>
        /// <param name="businessConnectionId">The business connection ID.</param>
        /// <param name="allowPaidBroadcast">Whether to allow paid broadcast.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The sent message.</returns>
        public static async Task<Message> Reply(
            this IAbstractHandlerContainer<Message> container,
            string text,
            ParseMode parseMode = ParseMode.None,
            ReplyMarkup? replyMarkup = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            int? messageThreadId = null,
            IEnumerable<MessageEntity>? entities = null,
            bool disableNotification = false,
            bool protectContent = false,
            string? messageEffectId = null,
            string? businessConnectionId = null,
            bool allowPaidBroadcast = false,
            CancellationToken cancellationToken = default)
            => await container.Client.SendMessage(
                container.ActualUpdate.Chat, text, parseMode, container.ActualUpdate,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, cancellationToken);

        /// <summary>
        /// Sends a response message to the current chat.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="text">The text of the message to send.</param>
        /// <param name="parseMode">The parse mode for the message text.</param>
        /// <param name="replyParameters">The reply parameters for the message.</param>
        /// <param name="replyMarkup">The reply markup for the message.</param>
        /// <param name="linkPreviewOptions">Options for link preview generation.</param>
        /// <param name="messageThreadId">The thread ID for forum topics.</param>
        /// <param name="entities">The message entities to include.</param>
        /// <param name="disableNotification">Whether to disable notification for the message.</param>
        /// <param name="protectContent">Whether to protect the message content.</param>
        /// <param name="messageEffectId">The message effect ID.</param>
        /// <param name="businessConnectionId">The business connection ID.</param>
        /// <param name="allowPaidBroadcast">Whether to allow paid broadcast.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The sent message.</returns>
        public static async Task<Message> Responce(
            this IAbstractHandlerContainer<Message> container,
            string text,
            ParseMode parseMode = ParseMode.None,
            ReplyParameters? replyParameters = null,
            ReplyMarkup? replyMarkup = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            int? messageThreadId = null,
            IEnumerable<MessageEntity>? entities = null,
            bool disableNotification = false,
            bool protectContent = false,
            string? messageEffectId = null,
            string? businessConnectionId = null,
            bool allowPaidBroadcast = false,
            CancellationToken cancellationToken = default)
            => await container.Client.SendMessage(
                container.ActualUpdate.Chat, text, parseMode, replyParameters,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, cancellationToken);

        /// <summary>
        /// Responnces to message that this CallbackQuery was originated from
        /// </summary>
        /// <param name="container"></param>
        /// <param name="text"></param>
        /// <param name="parseMode"></param>
        /// <param name="replyParameters"></param>
        /// <param name="replyMarkup"></param>
        /// <param name="linkPreviewOptions"></param>
        /// <param name="messageThreadId"></param>
        /// <param name="entities"></param>
        /// <param name="disableNotification"></param>
        /// <param name="protectContent"></param>
        /// <param name="messageEffectId"></param>
        /// <param name="businessConnectionId"></param>
        /// <param name="allowPaidBroadcast"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<Message> Responce(
            this IAbstractHandlerContainer<CallbackQuery> container,
            string text,
            ParseMode parseMode = ParseMode.None,
            ReplyParameters? replyParameters = null,
            ReplyMarkup? replyMarkup = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            int? messageThreadId = null,
            IEnumerable<MessageEntity>? entities = null,
            bool disableNotification = false,
            bool protectContent = false,
            string? messageEffectId = null,
            string? businessConnectionId = null,
            bool allowPaidBroadcast = false,
            CancellationToken cancellationToken = default)
        {
            CallbackQuery query = container.ActualUpdate;
            if (query.Message == null)
                throw new Exception("Callback origin message not found!");

            return await container.Client.SendMessage(
                query.Message.Chat, text, parseMode, replyParameters,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, cancellationToken);
        }
        
        /// <summary>
        /// Edits message text that this CallbackQuery was originated from
        /// </summary>
        /// <param name="container"></param>
        /// <param name="text"></param>
        /// <param name="parseMode"></param>
        /// <param name="replyMarkup"></param>
        /// <param name="entities"></param>
        /// <param name="linkPreviewOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<Message> EditMessage(
            this IAbstractHandlerContainer<CallbackQuery> container,
            string text,
            ParseMode parseMode = ParseMode.None,
            InlineKeyboardMarkup? replyMarkup = null,
            IEnumerable<MessageEntity>? entities = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            CancellationToken cancellationToken = default)
        {
            CallbackQuery query = container.ActualUpdate;
            if (query.Message == null)
                throw new Exception("Callback origin message not found!");

            return await container.Client.EditMessageText(
                query.Message.Chat,
                query.Message.MessageId,
                text: text,
                parseMode: parseMode,
                replyMarkup: replyMarkup,
                entities: entities,
                linkPreviewOptions: linkPreviewOptions,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Use this method to send answers to callback queries sent from <a href="https://core.telegram.org/bots/features#inline-keyboards">inline keyboards</a>.
        /// The answer will be displayed to the user as a notification at the top of the chat screen or as an alert
        /// </summary>
        /// <remarks>
        /// Alternatively, the user can be redirected to the specified Game URL.
        /// For this option to work, you must first create a game for your bot via <a href="https://t.me/botfather">@BotFather</a> and accept the terms.
        /// Otherwise, you may use links like <c>t.me/your_bot?start=XXXX</c> that open your bot with a parameter.
        /// </remarks>
        /// <param name="container"></param>
        /// <param name="text"></param>
        /// <param name="showAlert"></param>
        /// <param name="url"></param>
        /// <param name="cacheTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task AnswerCallbackQuery(
            this IAbstractHandlerContainer<CallbackQuery> container,
            string? text = null,
            bool showAlert = false,
            string? url = null,
            int cacheTime = 0,
            CancellationToken cancellationToken = default)
            => await container.Client.AnswerCallbackQuery(
                callbackQueryId: container.ActualUpdate.Id,
                text: text,
                showAlert: showAlert,
                url: url,
                cacheTime: cacheTime,
                cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Extensions methods for Awaiter Handler Builders
    /// </summary>
    public static class AwaiterHandlerBuilderExtensions
    {
        /// <summary>
        /// Awaits an update using the chat id key resolver and cancellation token.
        /// </summary>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="builder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TUpdate> ByChatId<TUpdate>(this IAwaiterHandlerBuilder<TUpdate> builder, CancellationToken cancellationToken = default) where TUpdate : class
            => await builder.Await(new ChatIdResolver(), cancellationToken);

        /// <summary>
        /// Awaits an update using the sender id key resolver and cancellation token.
        /// </summary>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="builder"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TUpdate> BySenderId<TUpdate>(this IAwaiterHandlerBuilder<TUpdate> builder, CancellationToken cancellationToken = default) where TUpdate : class
            => await builder.Await(new SenderIdResolver(), cancellationToken);
    }

    /// <summary>
    /// Extensions methods for awaiting providers
    /// Provides convenient methods for creating awaiter builders.
    /// </summary>
    public static class AwaitingProviderExtensions
    {
        /// <summary>
        /// Creates an awaiter handler builder for a specific update type.
        /// </summary>
        /// <typeparam name="TUpdate">The type of update to await.</typeparam>
        /// <param name="awaitingProvider"></param>
        /// <param name="updateType">The type of update to await.</param>
        /// <param name="handlingUpdate">The update that triggered the awaiter creation.</param>
        /// <returns>An awaiter handler builder for the specified update type.</returns>
        public static IAwaiterHandlerBuilder<TUpdate> CreateAbstract<TUpdate>(this IAwaitingProvider awaitingProvider, UpdateType updateType, Update handlingUpdate) where TUpdate : class
            => new AwaiterHandlerBuilder<TUpdate>(updateType, handlingUpdate, awaitingProvider);

        /// <summary>
        /// Creates an awaiter builder for any update type.
        /// </summary>
        /// <param name="awaitingProvider"></param>
        /// <param name="handlingUpdate"></param>
        /// <returns>An awaiter builder for any update type.</returns>
        public static IAwaiterHandlerBuilder<Update> AwaitAny(this IAwaitingProvider awaitingProvider, Update handlingUpdate)
            => awaitingProvider.CreateAbstract<Update>(UpdateType.Unknown, handlingUpdate);

        /// <summary>
        /// Creates an awaiter builder for message updates.
        /// </summary>
        /// <param name="awaitingProvider"></param>
        /// <param name="handlingUpdate"></param>
        /// <returns>An awaiter builder for message updates.</returns>
        public static IAwaiterHandlerBuilder<Message> AwaitMessage(this IAwaitingProvider awaitingProvider, Update handlingUpdate)
            => awaitingProvider.CreateAbstract<Message>(UpdateType.Message, handlingUpdate);

        /// <summary>
        /// Creates an awaiter builder for callback query updates.
        /// </summary>
        /// <param name="awaitingProvider"></param>
        /// <param name="handlingUpdate"></param>
        /// <returns>An awaiter builder for callback query updates.</returns>
        public static IAwaiterHandlerBuilder<CallbackQuery> AwaitCallbackQuery(this IAwaitingProvider awaitingProvider, Update handlingUpdate)
            => awaitingProvider.CreateAbstract<CallbackQuery>(UpdateType.CallbackQuery, handlingUpdate);
    }

    /// <summary>
    /// Extesions method for handlers providers
    /// </summary>
    public static class HandlersProviderExtensions
    {
        /// <summary>
        /// Gets the list of bot commands supported by the provider.
        /// </summary>
        /// <returns>An enumerable of bot commands.</returns>
        public static IEnumerable<BotCommand> GetBotCommands(this IHandlersProvider provider, CancellationToken cancellationToken = default)
        {
            if (!provider.TryGetDescriptorList(UpdateType.Message, out HandlerDescriptorList? list))
                yield break;

            foreach (BotCommand botCommand in list
                .Select(descriptor => descriptor.HandlerType)
                .SelectMany(handlerType => handlerType.GetCustomAttributes<CommandAlliasAttribute>()
                .SelectMany(attribute => attribute.Alliases.Select(alias => new BotCommand(alias, attribute.Description)))))
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return botCommand;
            }
        }
    }

    /// <summary>
    /// Extension methods for handlers collections.
    /// Provides convenient methods for creating implicit handlers.
    /// </summary>
    public static partial class HandlersCollectionExtensions
    {
        private static readonly string[] skippingAssemblies = ["System.", "Microsoft."];

        /// <summary>
        /// Collects all handlers from the current app domain.
        /// Scans for types that implement handlers and adds them to the collection.
        /// </summary>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the entry assembly cannot be found.</exception>
        public static IHandlersCollection CollectHandlersDomainWide(this IHandlersCollection handlers)
        {
            AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(ass => ass.GetName().Name != "Telegrator")
                .Where(ass => skippingAssemblies.All(skip => !ass.FullName.StartsWith(skip)))
                .SelectMany(ass => ass.GetExportedTypes())
                .Where(type => type.GetCustomAttribute<DontCollectAttribute>() == null)
                .Where(type => type.IsHandlerRealization())
                .ForEach(type => handlers.AddHandler(type));

            return handlers;
        }

        /// <summary>
        /// Collects all handlers from the calling this function assembly.
        /// Scans for types that implement handlers and adds them to the collection.
        /// </summary>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the entry assembly cannot be found.</exception>
        public static IHandlersCollection CollectHandlersAssemblyWide(this IHandlersCollection handlers)
        {
            Assembly.GetCallingAssembly()
                .GetExportedTypes()
                .Where(type => type.GetCustomAttribute<DontCollectAttribute>() == null)
                .Where(type => type.IsHandlerRealization())
                .ForEach(type => handlers.AddHandler(type));

            return handlers;
        }

        /// <summary>
        /// Creates a handler builder for a specific update type.
        /// </summary>
        /// <typeparam name="TUpdate">The type of update to handle.</typeparam>
        /// <param name="handlers">The handlers collection.</param>
        /// <param name="updateType">The type of update to handle.</param>
        /// <returns>A handler builder for the specified update type.</returns>
        public static HandlerBuilder<TUpdate> CreateHandler<TUpdate>(this IHandlersCollection handlers, UpdateType updateType) where TUpdate : class
            => new HandlerBuilder<TUpdate>(updateType, handlers);

        /// <summary>
        /// Creates a handler builder for any update type.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <returns>A handler builder for any update type.</returns>
        public static HandlerBuilder<Update> CreateAny(this IHandlersCollection handlers)
            => handlers.CreateHandler<Update>(UpdateType.Unknown);

        /// <summary>
        /// Creates a handler builder for message updates.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <returns>A handler builder for message updates.</returns>
        public static HandlerBuilder<Message> CreateMessage(this IHandlersCollection handlers)
            => handlers.CreateHandler<Message>(UpdateType.Message);

        /// <summary>
        /// Creates a handler builder for callback query updates.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <returns>A handler builder for callback query updates.</returns>
        public static HandlerBuilder<CallbackQuery> CreateCallbackQuery(this IHandlersCollection handlers)
            => handlers.CreateHandler<CallbackQuery>(UpdateType.CallbackQuery);

        /// <summary>
        /// Creates implicit handler from method
        /// </summary>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IHandlersCollection AddMethod<TUpdate>(this IHandlersCollection handlers, AbstractHandlerAction<TUpdate> method) where TUpdate : class
        {
            MethodHandlerDescriptor<TUpdate> descriptor = new MethodHandlerDescriptor<TUpdate>(method);
            handlers.AddDescriptor(descriptor);
            return handlers;
        }
    }

    /// <summary>
    /// Extension methods for handler builders.
    /// Provides convenient methods for creating handlers and setting state keepers.
    /// </summary>
    public static partial class HandlerBuilderExtensions
    {
        /// <inheritdoc cref="HandlerBuilderBase.SetUpdateValidating(UpdateValidateAction)"/>
        public static TBuilder SetUpdateValidating<TBuilder>(this TBuilder handlerBuilder, UpdateValidateAction updateValidateAction)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetUpdateValidating(updateValidateAction);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.SetConcurreny(int)"/>
        public static TBuilder SetConcurreny<TBuilder>(this TBuilder handlerBuilder, int concurrency)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetConcurreny(concurrency);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.SetPriority(int)"/>
        public static TBuilder SetPriority<TBuilder>(this TBuilder handlerBuilder, int priority)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetPriority(priority);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.SetIndexer(int, int)"/>
        public static TBuilder SetIndexer<TBuilder>(this TBuilder handlerBuilder, int concurrency, int priority)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetIndexer(concurrency, priority);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.AddFilter(IFilter{Update})"/>
        public static TBuilder AddFilter<TBuilder>(this TBuilder handlerBuilder, IFilter<Update> filter)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.AddFilter(filter);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.AddFilters(IFilter{Update}[])"/>
        public static TBuilder AddFilters<TBuilder>(this TBuilder handlerBuilder, params IFilter<Update>[] filters)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.AddFilters(filters);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.SetStateKeeper{TKey, TState, TKeeper}(TState, IStateKeyResolver{TKey})"/>
        public static TBuilder SetStateKeeper<TBuilder, TKey, TState, TKeeper>(this TBuilder handlerBuilder, TState myState, IStateKeyResolver<TKey> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            handlerBuilder.SetStateKeeper<TKey, TState, TKeeper>(myState, keyResolver);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.SetStateKeeper{TKey, TState, TKeeper}(SpecialState, IStateKeyResolver{TKey})"/>
        public static TBuilder SetStateKeeper<TBuilder, TKey, TState, TKeeper>(this TBuilder handlerBuilder, SpecialState specialState, IStateKeyResolver<TKey> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            handlerBuilder.SetStateKeeper<TKey, TState, TKeeper>(specialState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Adds a targeted filter for a specific filter target type.
        /// </summary>
        /// <typeparam name="TBuilder"></typeparam>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="handlerBuilder"></param>
        /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
        /// <param name="filter">The filter to add.</param>
        /// <returns>The builder instance.</returns>
        public static TBuilder AddTargetedFilter<TBuilder, TFilterTarget>(this TBuilder handlerBuilder, Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter)
            where TBuilder : HandlerBuilderBase
            where TFilterTarget : class
        {
            handlerBuilder.AddTargetedFilter(getFilterringTarget, filter);
            return handlerBuilder;
        }

        /// <summary>
        /// Adds multiple targeted filters for a specific filter target type.
        /// </summary>
        /// <typeparam name="TBuilder"></typeparam>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="handlerBuilder"></param>
        /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The builder instance.</returns>
        public static TBuilder AddTargetedFilters<TBuilder, TFilterTarget>(this TBuilder handlerBuilder, Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters)
            where TBuilder : HandlerBuilderBase
            where TFilterTarget : class
        {
            handlerBuilder.AddTargetedFilters(getFilterringTarget, filters);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets a numeric state keeper with a custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The numeric state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, int myState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(myState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets a numeric state keeper with a special state and custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, SpecialState specialState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(specialState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets a numeric state keeper with the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The numeric state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, int myState)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(myState, new SenderIdResolver());
            return handlerBuilder;
        }

        /// <summary>
        /// Sets a numeric state keeper with a special state and the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, SpecialState specialState)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(specialState, new SenderIdResolver());
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with a custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The enum state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, TEnum myState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(myState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with a special state and custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, SpecialState specialState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(specialState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The enum state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, TEnum myState)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(myState, new SenderIdResolver());
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with a special state and the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, SpecialState specialState)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(specialState, new SenderIdResolver());
            return handlerBuilder;
        }
    }

    /// <summary>
    /// Provides extension methods for working with collections.
    /// </summary>
    public static partial class ColletionsExtensions
    {
        /// <summary>
        /// Creates a <see cref="ReadOnlyDictionary{TKey, TValue}"/> from an <see cref="IEnumerable{TValue}"/>
        /// according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector) where TKey : notnull
        {
            Dictionary<TKey, TValue> dictionary = source.ToDictionary(keySelector);
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Enumerates objects in a <paramref name="source"/> and executes an <paramref name="action"/> on each one
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<TValue> ForEach<TValue>(this IEnumerable<TValue> source, Action<TValue> action)
        {
            foreach (TValue value in source)
                action.Invoke(value);

            return source;
        }

        /// <summary>
        /// Creates a new <see cref="IEnumerable{T}"/> with the elements of the <paramref name="source"/> that were successfully cast to the <typeparamref name="TResult"/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> WhereCast<TResult>(this IEnumerable source)
        {
            foreach (object value in source)
            {
                if (value is TResult result)
                    yield return result;
            }
        }

        /// <summary>
        /// Sets the value of a key in a dictionary, or if the key does not exist, adds it
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }

        /// <summary>
        /// Sets the value of a key in a dictionary, or if the key does not exist, adds its default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value, TValue defaultValue)
        {
            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, defaultValue);
        }

        /// <summary>
        /// Return the random object from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource Random<TSource>(this IEnumerable<TSource> source)
            => source.Random(new Random());

        /// <summary>
        /// Return the random object from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static TSource Random<TSource>(this IEnumerable<TSource> source, Random random)
            => source.ElementAt(random.Next(0, source.Count() - 1));

        /// <summary>
        /// Adds a range of elements to collection if they dont already exist using default equality comparer
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="list"></param>
        /// <param name="elements"></param>
        public static void Union<TSource>(this IList<TSource> list, params IEnumerable<TSource> elements)
        {
            foreach (TSource item in elements)
            {
                if (!list.Contains(item, EqualityComparer<TSource>.Default))
                    list.Add(item);
            }
        }
    }

    /// <summary>
    /// Provides extension methods for reflection and type inspection.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        /// <summary>
        /// Checks if a type implements the <see cref="ICustomDescriptorsProvider"/> interface.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type implements ICustomDescriptorsProvider; otherwise, false.</returns>
        public static bool IsCustomDescriptorsProvider(this Type type)
            => type.GetInterface(nameof(ICustomDescriptorsProvider)) != null;

        /// <summary>
        /// Checks if <paramref name="type"/> is a <see cref="IFilter{T}"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFilterType(this Type type)
            => type.IsAssignableToGenericType(typeof(IFilter<>));

        /// <summary>
        /// Checks if <paramref name="type"/> is a descendant of <see cref="UpdateHandlerBase"/> class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHandlerAbstract(this Type type)
            => type.IsAbstract && typeof(UpdateHandlerBase).IsAssignableFrom(type);

        /// <summary>
        /// Checks if <paramref name="type"/> is an implementation of <see cref="UpdateHandlerBase"/> class or its descendants
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHandlerRealization(this Type type)
            => !type.IsAbstract && type != typeof(UpdateHandlerBase) && typeof(UpdateHandlerBase).IsAssignableFrom(type);

        /// <summary>
        /// Checks if <paramref name="type"/> has a parameterless constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasParameterlessCtor(this Type type)
            => type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0);

        /*
        /// <summary>
        /// Invokes a "<paramref name="methodName"/>" method of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object? InvokeMethod(this object obj, string methodName, params object[]? args)
            => obj.GetType().GetMethod(methodName, BindAll).InvokeMethod(obj, args);

        /// <summary>
        /// Invokes a method of <paramref name="methodInfo"/>
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object? InvokeMethod(this MethodInfo methodInfo, object obj, params object[]? args)
            => methodInfo.Invoke(obj, args);

        /// <summary>
        /// Invokes a static method of <paramref name="methodInfo"/>
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object? InvokeStaticMethod(this MethodInfo methodInfo, params object[]? parameters)
            => methodInfo.Invoke(null, parameters);

        /// <summary>
        /// Invokes a static "<paramref name="methodName"/>" method of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T? InvokeStaticMethod<T>(this object obj, string methodName, params object[]? parameters)
            => (T?)obj.GetType().GetMethod(methodName, BindAll).InvokeStaticMethod(parameters);

        /// <summary>
        /// Invokes a generic method of <paramref name="methodInfo"/> with generic types in <paramref name="genericParameters"/>
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="obj"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeGenericMethod(this MethodInfo methodInfo, object obj, Type[] genericParameters, params object[]? parameters)
            => methodInfo.MakeGenericMethod(genericParameters).Invoke(obj, parameters);

        /// <summary>
        /// Invokes a generic <paramref name="methodName"/> method with generic types in <paramref name="genericParameters"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="genericParameters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T? InvokeGenericMethod<T>(this object obj, string methodName, Type[] genericParameters, params object[]? parameters)
            => (T?)obj.GetType().GetMethod(methodName).InvokeGenericMethod(obj, genericParameters, parameters);
        */

        /// <summary>
        /// Checks is <paramref name="obj"/> has public properties
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasPublicProperties(this object obj)
            => obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.Name != "IsCollectible").Any();

        /// <summary>
        /// Determines whether an instance of a specified type can be assigned to an instance of the current type
        /// </summary>
        /// <param name="givenType"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == genericType))
                return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            if (givenType.BaseType == null)
                return false;

            return givenType.BaseType.IsAssignableToGenericType(genericType);
        }
    }

    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Slices a <paramref name="source"/> string into a array of substrings of fixed <paramref name="length"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<string> SliceBy(this string source, int length)
        {
            for (int start = 0; start < source.Length; start += length + 1)
            {
                int tillEnd = source.Length - start;
                int toSlice = tillEnd < length + 1 ? tillEnd : length + 1;

                ReadOnlySpan<char> chunk = source.AsSpan().Slice(start, toSlice);
                yield return chunk.ToString();
            }
        }
    }

    /// <summary>
    /// Provides extension methods for working with Telegram Update objects.
    /// </summary>
    public static partial class UpdateExtensions
    {
        /// <summary>
        /// Selects from Update an object from which you can get the sender's ID
        /// </summary>
        /// <param name="update"></param>
        /// <returns>Sender's ID</returns>
        public static long? GetSenderId(this Update update) => update switch
        {
            { Message.From: { } from } => from.Id,
            { Message.SenderChat: { } chat } => chat.Id,
            { EditedMessage.From: { } from } => from.Id,
            { EditedMessage.SenderChat: { } chat } => chat.Id,
            { ChannelPost.From: { } from } => from.Id,
            { ChannelPost.SenderChat: { } chat } => chat.Id,
            { EditedChannelPost.From: { } from } => from.Id,
            { EditedChannelPost.SenderChat: { } chat } => chat.Id,
            { CallbackQuery.From: { } from } => from.Id,
            { InlineQuery.From: { } from } => from.Id,
            { PollAnswer.User: { } user } => user.Id,
            { PreCheckoutQuery.From: { } from } => from.Id,
            { ShippingQuery.From: { } from } => from.Id,
            { ChosenInlineResult.From: { } from } => from.Id,
            { ChatJoinRequest.From: { } from } => from.Id,
            { ChatMember.From: { } from } => from.Id,
            { MyChatMember.From: { } from } => from.Id,
            _ => null
        };

        /// <summary>
        /// Selects from Update an object from which you can get the chat's ID
        /// </summary>
        /// <param name="update"></param>
        /// <returns>Sender's ID</returns>
        public static long? GetChatId(this Update update) => update switch
        {
            { Message.Chat: { } chat } => chat.Id,
            { Message.SenderChat: { } chat } => chat.Id,
            { EditedMessage.Chat: { } chat } => chat.Id,
            { EditedMessage.SenderChat: { } chat } => chat.Id,
            { ChannelPost.Chat: { } chat } => chat.Id,
            { ChannelPost.SenderChat: { } chat } => chat.Id,
            { EditedChannelPost.Chat: { } chat } => chat.Id,
            { EditedChannelPost.SenderChat: { } chat } => chat.Id,
            { CallbackQuery.Message.Chat: { } chat } => chat.Id,
            { ChatJoinRequest.Chat: { } chat } => chat.Id,
            { ChatMember.Chat: { } chat } => chat.Id,
            { MyChatMember.Chat: { } chat } => chat.Id,
            _ => null
        };

        /// <summary>
        /// Selects from <see cref="Update"/> an object that contains information about the update
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public static object GetActualUpdateObject(this Update update) => update switch
        {
            { Message: { } message } => message,
            { EditedMessage: { } editedMessage } => editedMessage,
            { ChannelPost: { } channelPost } => channelPost,
            { EditedChannelPost: { } editedChannelPost } => editedChannelPost,
            { BusinessConnection: { } businessConnection } => businessConnection,
            { BusinessMessage: { } businessMessage } => businessMessage,
            { EditedBusinessMessage: { } editedBusinessMessage } => editedBusinessMessage,
            { DeletedBusinessMessages: { } deletedBusinessMessages } => deletedBusinessMessages,
            { MessageReaction: { } messageReaction } => messageReaction,
            { MessageReactionCount: { } messageReactionCount } => messageReactionCount,
            { InlineQuery: { } inlineQuery } => inlineQuery,
            { ChosenInlineResult: { } chosenInlineResult } => chosenInlineResult,
            { CallbackQuery: { } callbackQuery } => callbackQuery,
            { ShippingQuery: { } shippingQuery } => shippingQuery,
            { PreCheckoutQuery: { } preCheckoutQuery } => preCheckoutQuery,
            { PurchasedPaidMedia: { } purchasedPaidMedia } => purchasedPaidMedia,
            { Poll: { } poll } => poll,
            { PollAnswer: { } pollAnswer } => pollAnswer,
            { MyChatMember: { } myChatMember } => myChatMember,
            { ChatMember: { } chatMember } => chatMember,
            { ChatJoinRequest: { } chatJoinRequest } => chatJoinRequest,
            { ChatBoost: { } chatBoost } => chatBoost,
            { RemovedChatBoost: { } removedChatBoost } => removedChatBoost,
            _ => update
        };

        /// <summary>
        /// Selecting corresponding <see cref="UpdateType"/>s for <see cref="Update"/>'s sub-type
        /// </summary>
        /// <returns></returns>
        public static UpdateType[] GetAllowedUpdateTypes(this Type type) => type.FullName switch
        {
            "Telegram.Bot.Types.Message" => UpdateTypeExtensions.MessageTypes,
            "Telegram.Bot.Types.ChatMemberUpdated" => [UpdateType.MyChatMember, UpdateType.ChatMember],
            "Telegram.Bot.Types.InlineQuery" => [UpdateType.InlineQuery],
            "Telegram.Bot.Types.ChosenInlineResult" => [UpdateType.ChosenInlineResult],
            "Telegram.Bot.Types.CallbackQuery" => [UpdateType.CallbackQuery],
            "Telegram.Bot.Types.ShippingQuery" => [UpdateType.ShippingQuery],
            "Telegram.Bot.Types.PreCheckoutQuery" => [UpdateType.PreCheckoutQuery],
            "Telegram.Bot.Types.Poll" => [UpdateType.Poll],
            "Telegram.Bot.Types.PollAnswer" => [UpdateType.PollAnswer],
            "Telegram.Bot.Types.ChatJoinRequest" => [UpdateType.ChatJoinRequest],
            "Telegram.Bot.Types.MessageReactionUpdated" => [UpdateType.MessageReaction],
            "Telegram.Bot.TypesMessageReactionCountUpdated" => [UpdateType.MessageReactionCount],
            "Telegram.Bot.Types.ChatBoostUpdated" => [UpdateType.ChatBoost],
            "Telegram.Bot.Types.ChatBoostRemoved" => [UpdateType.RemovedChatBoost],
            "Telegram.Bot.Types.BusinessConnection" => [UpdateType.BusinessConnection],
            "Telegram.Bot.Types.BusinessMessagesDeleted" => [UpdateType.DeletedBusinessMessages],
            "Telegram.Bot.Types.PaidMediaPurchased" => [UpdateType.PurchasedPaidMedia],
            "Telegram.Bot.Types.Update" => Update.AllTypes,
            _ => []
        };

        /// <summary>
        /// Selecting corresponding <see cref="UpdateType"/>s for <see cref="Update"/>'s sub-type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static UpdateType[] GetAllowedUpdateTypes<T>() where T : class
            => GetAllowedUpdateTypes(typeof(T));

        /// <summary>
        /// Selects from <see cref="Update"/> an <typeparamref name="T"/> that contains information about the update
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public static T GetActualUpdateObject<T>(this Update update)
        {
            object actualUpdate = update.GetActualUpdateObject() ?? throw new Exception();
            if (actualUpdate is not T actualCasted)
                throw new Exception();

            return actualCasted;
        }
    }

    /// <summary>
    /// Provides extension methods for working with UpdateType enums.
    /// </summary>
    public static partial class UpdateTypeExtensions
    {
        /// <summary>
        /// <see cref="UpdateType"/>'s that contain a message
        /// </summary>
        public static readonly UpdateType[] MessageTypes =
        [
            UpdateType.Message,
            UpdateType.EditedMessage,
            UpdateType.BusinessMessage,
            UpdateType.EditedBusinessMessage,
            UpdateType.ChannelPost,
            UpdateType.EditedChannelPost
        ];

        /// <summary>
        /// Checks if <typeparamref name="T"/> matches one of the <see cref="UpdateType"/>'s give on <paramref name="allowedTypes"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allowedTypes"></param>
        /// <returns></returns>
        public static bool IsUpdateObjectAllowed<T>(this UpdateType[] allowedTypes) where T : class
        {
            return allowedTypes.Any(t => t.IsValidUpdateObject<T>());
        }

        /// <summary>
        /// Checks if <typeparamref name="TUpdate"/> matches the given <see cref="UpdateType"/>
        /// </summary>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="updateType"></param>
        /// <returns></returns>
        public static bool IsValidUpdateObject<TUpdate>(this UpdateType updateType) where TUpdate : class
        {
            if (typeof(TUpdate) == typeof(Update))
                return true;

            return typeof(TUpdate).Equals(updateType.ReflectUpdateObject());
        }

        /// <summary>
        /// Returns an update object corresponding to the <see cref="UpdateType"/>.
        /// </summary>
        /// <param name="updateType"></param>
        /// <returns></returns>
        public static Type? ReflectUpdateObject(this UpdateType updateType)
        {
            return updateType switch
            {
                UpdateType.Message or UpdateType.EditedMessage or UpdateType.BusinessMessage or UpdateType.EditedBusinessMessage or UpdateType.ChannelPost or UpdateType.EditedChannelPost => typeof(Message),
                UpdateType.MyChatMember => typeof(ChatMemberUpdated),
                UpdateType.ChatMember => typeof(ChatMemberUpdated),
                UpdateType.InlineQuery => typeof(InlineQuery),
                UpdateType.ChosenInlineResult => typeof(ChosenInlineResult),
                UpdateType.CallbackQuery => typeof(CallbackQuery),
                UpdateType.ShippingQuery => typeof(ShippingQuery),
                UpdateType.PreCheckoutQuery => typeof(PreCheckoutQuery),
                UpdateType.Poll => typeof(Poll),
                UpdateType.PollAnswer => typeof(PollAnswer),
                UpdateType.ChatJoinRequest => typeof(ChatJoinRequest),
                UpdateType.MessageReaction => typeof(MessageReactionUpdated),
                UpdateType.MessageReactionCount => typeof(MessageReactionCountUpdated),
                UpdateType.ChatBoost => typeof(ChatBoostUpdated),
                UpdateType.RemovedChatBoost => typeof(ChatBoostRemoved),
                UpdateType.BusinessConnection => typeof(BusinessConnection),
                UpdateType.DeletedBusinessMessages => typeof(BusinessMessagesDeleted),
                UpdateType.PurchasedPaidMedia => typeof(PaidMediaPurchased),
                _ or UpdateType.Unknown => typeof(Update)
            };
        }
    }

    /// <summary>
    /// Contains extension method for number types
    /// </summary>
    public static class NumbersExtensions
    {
        /// <summary>
        /// Check if int value has int flag using bit compare
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlag(this int value, int flag)
            => (value & flag) == flag;

        /// <summary>
        /// Check if int value has enum flag using bit compare
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this int value, T flag) where T : Enum
            => value.HasFlag(Convert.ToInt32(flag));
    }
}
