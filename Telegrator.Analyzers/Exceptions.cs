namespace Telegrator.Analyzers
{
    /// <summary>
    /// Exception thrown when a target is not found during code generation.
    /// </summary>
    internal class TargteterNotFoundException() : Exception() { }
    
    /// <summary>
    /// Exception thrown when a base class type is not found during code generation.
    /// </summary>
    internal class BaseClassTypeNotFoundException() : Exception() { }
}
