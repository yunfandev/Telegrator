using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using System.Text;

namespace Telegrator.Analyzers
{
    internal static class TypeExtensions
    {
        public static StringBuilder AppendTabs(this StringBuilder builder, int count)
            => builder.Append(new string('\t', count));

        public static string GetBaseTypeSyntaxName(this BaseTypeSyntax baseClassSyntax)
        {
            if (baseClassSyntax is PrimaryConstructorBaseTypeSyntax parimaryConstructor)
                return parimaryConstructor.Type.ToString();

            if (baseClassSyntax is SimpleBaseTypeSyntax simpleBaseType)
                return simpleBaseType.Type.ToString();

            throw new BaseClassTypeNotFoundException();
        }

        public static bool IsAssignableFrom(this ITypeSymbol? symbol, string className)
        {
            if (symbol is null)
                return false;

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

        public static IEnumerable<TResult> WhereCast<TResult>(this IEnumerable source)
        {
            foreach (object value in source)
            {
                if (value is TResult result)
                    yield return result;
            }
        }

        public static CompilationUnitSyntax FindCompilationUnitSyntax(this SyntaxNode syntax)
        {
            while (syntax is not CompilationUnitSyntax)
                syntax = syntax.Parent ?? throw new Exception();

            return (CompilationUnitSyntax)syntax;
        }

        public static IEnumerable<TSource> IntersectBy<TSource, TValue>(this IEnumerable<TSource> first, IEnumerable<TValue> second, Func<TSource, TValue> selector)
        {
            foreach (TSource item in first)
            {
                TValue value = selector(item);
                if (second.Contains(value))
                    yield return item;
            }
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
