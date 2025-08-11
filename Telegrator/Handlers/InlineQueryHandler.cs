using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegrator.Attributes;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Handlers
{
    public class InlineQueryHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<InlineQueryHandler>(UpdateType.InlineQuery, importance)
    {
        public override bool CanPass(FilterExecutionContext<Update> context) => context.Input.InlineQuery is { } | context.Input.ChosenInlineResult is { };
    }
     
    public abstract class InlineQueryHandler() : AbstractUpdateHandler<Update>(UpdateType.InlineQuery)
    {
        protected IAbstractHandlerContainer<InlineQuery> QueryContainer { get; private set; } = null!;

        protected IAbstractHandlerContainer<ChosenInlineResult> ChosenContainer { get; private set; } = null!;

        protected InlineQuery InputQuery { get; private set; } = null!;

        protected ChosenInlineResult InputChosen { get; private set; } = null!;

        public override async Task<Result> Execute(IAbstractHandlerContainer<Update> container, CancellationToken cancellation)
        {
            switch (container.HandlingUpdate.Type)
            {
                case UpdateType.InlineQuery:
                    {
                        QueryContainer = AbstractHandlerContainer<InlineQuery>.From(container);
                        InputQuery = QueryContainer.ActualUpdate;
                        return await Requested(QueryContainer, cancellation);
                    }

                case UpdateType.ChosenInlineResult:
                    {
                        ChosenContainer = AbstractHandlerContainer<ChosenInlineResult>.From(container);
                        InputChosen = ChosenContainer.ActualUpdate;
                        return await Chosen(ChosenContainer, cancellation);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        public abstract Task<Result> Requested(IAbstractHandlerContainer<InlineQuery> container, CancellationToken cancellation);

        public abstract Task<Result> Chosen(IAbstractHandlerContainer<ChosenInlineResult> container, CancellationToken cancellation);

        protected async Task Answer(
            IEnumerable<InlineQueryResult> results,
            int? cacheTime = null,
            bool isPersonal = false,
            string? nextOffset = null,
            InlineQueryResultsButton? button = null,
            CancellationToken cancellationToken = default)
            => await QueryContainer.AnswerInlineQuery(
                results, cacheTime,
                isPersonal, nextOffset,
                button, cancellationToken);
    }
}
