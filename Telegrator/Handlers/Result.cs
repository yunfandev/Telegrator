namespace Telegrator.Handlers
{
    /// <summary>
    /// Represents handler results, allowing to communicate with router and control aspect execution
    /// </summary>
    public sealed class Result
    {
        /// <summary>
        /// Is result positive
        /// </summary>
        public bool Positive { get; }

        /// <summary>
        /// Should router search for next matching handler
        /// </summary>
        public bool RouteNext { get; }

        /// <summary>
        /// Exact type that router should search
        /// </summary>
        public Type? NextType { get; }

        internal Result(bool positive, bool routeNext, Type? nextType)
        {
            Positive = positive;
            RouteNext = routeNext;
            NextType = nextType;
        }

        /// <summary>
        /// "OK" result
        /// </summary>
        /// <returns></returns>
        public static Result Ok()
            => new Result(true, false, null);

        /// <summary>
        /// "Somethong went wrong" result
        /// </summary>
        /// <returns></returns>
        public static Result Fault()
            => new Result(false, false, null);

        /// <summary>
        /// "Search next handler" result
        /// </summary>
        /// <returns></returns>
        public static Result Next()
            => new Result(true, true, null);

        /// <summary>
        /// "Search next handler of type" result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Result Next<T>()
            => new Result(true, true, typeof(T));
    }
}
