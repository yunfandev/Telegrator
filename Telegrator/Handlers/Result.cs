namespace Telegrator.Handlers
{
    public sealed class Result
    {
        public bool Positive { get; }

        public bool RouteNext { get; }

        public Type? NextType { get; }

        internal Result(bool positive, bool routeNext, Type? nextType)
        {
            Positive = positive;
            RouteNext = routeNext;
            NextType = nextType;
        }

        public static Result Ok()
            => new Result(true, false, null);

        public static Result Fault()
            => new Result(false, false, null);

        public static Result Next()
            => new Result(true, true, null);

        public static Result Next<T>()
            => new Result(true, true, typeof(T));
    }
}
