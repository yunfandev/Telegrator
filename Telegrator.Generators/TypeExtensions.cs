using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telegrator.Generators
{
    internal static partial class TypeExtensions
    {
        public static INamedTypeSymbol TryGetNamedType(this BaseTypeDeclarationSyntax syntax, Compilation compilation)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            return semanticModel.GetDeclaredSymbol(syntax)!;
        }

        public static string GetBaseTypeSyntaxName(this BaseTypeSyntax baseClassSyntax)
        {
            if (baseClassSyntax is PrimaryConstructorBaseTypeSyntax parimaryConstructor)
                return parimaryConstructor.Type.ToString();

            if (baseClassSyntax is SimpleBaseTypeSyntax simpleBaseType)
                return simpleBaseType.Type.ToString();

            throw new BaseClassTypeNotFoundException();
        }

        public static bool IsAssignableFrom(this ITypeSymbol symbol, string className)
        {
            if (symbol.BaseType == null)
                return false;
                
            if (symbol.BaseType.Name == className)
                return true;

            return symbol.BaseType.IsAssignableFrom(className);
        }

        public static ITypeSymbol? Cast(this ITypeSymbol symbol, string className)
        {
            if (symbol.BaseType == null)
                return null;

            if (symbol.BaseType.Name == className)
                return symbol.BaseType;

            return symbol.BaseType.Cast(className);
        }

        public static CompilationUnitSyntax FindCompilationUnitSyntax(this SyntaxNode syntax)
        {
            while (syntax is not CompilationUnitSyntax)
                syntax = syntax.Parent ?? throw new Exception();

            return (CompilationUnitSyntax)syntax;
        }

        public static IList<TValue> UnionAdd<TValue>(this IList<TValue> source, IEnumerable<TValue> toUnion)
        {
            foreach (TValue toUnionValue in toUnion)
            {
                if (!source.Contains(toUnionValue, EqualityComparer<TValue>.Default))
                    source.Add(toUnionValue);
            }

            return source;
        }
    }
}
