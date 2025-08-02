namespace Telegrator.Aspects
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AfterExecutionAttribute<T> : Attribute where T : IPostProcessor
    {
        public Type ProcessorType => typeof(T);
    }
}
