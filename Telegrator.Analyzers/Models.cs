using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telegrator.Analyzers
{
    internal class HandlerDeclarationModel(ClassDeclarationSyntax classDeclaration, IEnumerable<AttributeSyntax> handlerAttributes, BaseTypeSyntax? baseType)
    {
        public ClassDeclarationSyntax ClassDeclaration { get; } = classDeclaration;
        public IEnumerable<AttributeSyntax> HandlerAttributes { get; } = handlerAttributes;
        public BaseTypeSyntax? BaseType { get; } = baseType;
        public bool HasAttributes => HandlerAttributes.Any();
    }
}
