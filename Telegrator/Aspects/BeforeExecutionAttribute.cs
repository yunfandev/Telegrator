namespace Telegrator.Aspects
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BeforeExecutionAttribute<T> : Attribute where T : IPreProcessor
    {
        public Type ProcessorType => typeof(T);
    }
}
