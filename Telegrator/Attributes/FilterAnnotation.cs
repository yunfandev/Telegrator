using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters;
using Telegrator.Filters.Components;

namespace Telegrator.Attributes
{
    /// <summary>
    /// Reactive way to implement a new <see cref="UpdateFilterAttribute{T}"/> of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FilterAnnotation<T> : UpdateFilterAttribute<T>, IFilter<T> where T : class
    {
        /// <inheritdoc/>
        public bool IsCollectible => false;

        /// <summary>
        /// Initializes new instance of <see cref="FilterAnnotation{T}"/>
        /// </summary>
        public FilterAnnotation() : base()
        {
            UpdateFilter = Filter<T>.If(CanPass);
            AnonymousFilter = AnonymousTypeFilter.Compile(UpdateFilter, GetFilterringTarget);
        }

        /// <inheritdoc/>
        public abstract bool CanPass(FilterExecutionContext<T> context);
    }

    /// <inheritdoc/>
    public abstract class MessageFilterAnnotation() : FilterAnnotation<Message>()
    {
        /// <inheritdoc/>
        public override UpdateType[] AllowedTypes => [UpdateType.Message, UpdateType.EditedMessage, UpdateType.ChannelPost, UpdateType.EditedChannelPost, UpdateType.BusinessMessage, UpdateType.EditedBusinessMessage];
        
        /// <inheritdoc/>
        public override Message? GetFilterringTarget(Update update) => update.Message;
    }

    /// <inheritdoc/>
    public abstract class CallbackQueryFilterAnnotation() : FilterAnnotation<CallbackQuery>()
    {
        /// <inheritdoc/>
        public override UpdateType[] AllowedTypes => [UpdateType.CallbackQuery];

        /// <inheritdoc/>
        public override CallbackQuery? GetFilterringTarget(Update update) => update.CallbackQuery;
    }
}
