﻿using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegrator.Annotations;
using Telegrator.Attributes;
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

        /// <summary>
        /// Checkes if sent <see cref="Message"/> contains command. Automatically cuts bot name from it
        /// </summary>
        /// <param name="message"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool IsCommand(this Message message, out string? command)
        {
            command = null;
            if (message is not { Entities.Length: > 0, Text.Length: > 0 })
                return false;

            MessageEntity commandEntity = message.Entities[0];
            if (commandEntity.Type != MessageEntityType.BotCommand)
                return false;

            if (commandEntity.Offset != 0)
                return false;

            command = message.Text.Substring(1, commandEntity.Length - 1);
            if (command.Contains('@'))
            {
                string[] split = command.Split('@');
                command = split[0];
            }

            return true;
        }

        /// <summary>
        /// Split message text into arguments, ignoring command instance. Splits by space character
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MissingMemberException"></exception>
        public static string[] SplitArgs(this Message message)
        {
            if (!message.IsCommand(out _))
                throw new InvalidDataException("Message does not contain a command");

            if (message is not { Text.Length: > 0 })
                throw new ArgumentNullException(nameof(message), "Command text cannot be null or empty");

            if (!message.Text.Contains(' '))
                throw new MissingMemberException("Command dont contains arguments");

            return message.Text.Split([' '], StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        }

        /// <summary>
        /// Tries to split message text into arguments, ignoring command instance. Splits by space character. Exception-free version of <see cref="SplitArgs(Message)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool TrySplitArgs(this Message message, out string[]? args)
        {
            args = null;
            if (!message.IsCommand(out _))
                return false;

            if (message is not { Text.Length: > 0 })
                return false;

            if (!message.Text.Contains(' '))
                return false;

            args = message.Text.Split([' '], StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            return true;
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
        /// Adds a handler type to the collection.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <typeparam name="THandler">The type of handler to add.</typeparam>
        /// <returns>This collection instance for method chaining.</returns>
        public static IHandlersCollection AddHandler<THandler>(this IHandlersCollection handlers) where THandler : UpdateHandlerBase
            => handlers.AddHandler(typeof(THandler));

        /// <summary>
        /// Adds a handler type to the collection.
        /// </summary>
        /// <param name="handlers">The handlers collection.</param>
        /// <param name="handlerType">The type of handler to add.</param>
        /// <returns>This collection instance for method chaining.</returns>
        /// <exception cref="Exception">Thrown when the type is not a valid handler implementation.</exception>
        public static IHandlersCollection AddHandler(this IHandlersCollection handlers, Type handlerType)
        {
            if (!handlerType.IsHandlerRealization())
                throw new Exception();

            if (handlerType.IsCustomDescriptorsProvider())
            {
                ICustomDescriptorsProvider provider = (ICustomDescriptorsProvider)Activator.CreateInstance(handlerType);
                foreach (HandlerDescriptor handlerDescriptor in provider.DescribeHandlers())
                    handlers.AddDescriptor(handlerDescriptor);
            }
            else
            {
                HandlerDescriptor descriptor = new HandlerDescriptor(DescriptorType.General, handlerType);
                handlers.AddDescriptor(descriptor);
            }

            return handlers;
        }

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
            if (update is T upd)
                return upd;

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
        /// Dictionary of <see cref="UpdateType"/>s that suppresses to generic type for handling types that has complex multi-type handlers
        /// </summary>
        public static readonly Dictionary<UpdateType, UpdateType> SuppressTypes = new Dictionary<UpdateType, UpdateType>()
        {
            { UpdateType.ChosenInlineResult, UpdateType.InlineQuery }
        };

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
}
