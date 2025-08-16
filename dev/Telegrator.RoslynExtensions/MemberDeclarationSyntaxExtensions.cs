using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Telegrator.RoslynExtensions
{
    public static class MemberDeclarationSyntaxExtensions
    {
        private static SyntaxTrivia TabulationTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, "\t");
        private static SyntaxTrivia WhitespaceTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");
        private static SyntaxTrivia NewLineTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n");
        private static SyntaxToken Semicolon => SyntaxFactory.Token(SyntaxKind.SemicolonToken);

        public static SyntaxTokenList Decorate(this SyntaxTokenList tokens)
            => new SyntaxTokenList(tokens.Select(token => token.WithoutTrivia().WithTrailingTrivia(WhitespaceTrivia)).ToArray());

        public static T DecorateMember<T>(this T typeDeclaration, int times = 1) where T : MemberDeclarationSyntax => typeDeclaration
            .WithoutTrivia().WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia);

        public static NamespaceDeclarationSyntax Decorate(this NamespaceDeclarationSyntax namespaceDeclaration) => namespaceDeclaration
            .WithName(namespaceDeclaration.Name.WithoutTrivia().WithLeadingTrivia(WhitespaceTrivia))
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(NewLineTrivia).WithTrailingTrivia(NewLineTrivia))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        public static T Decorate<T>(this T typeDeclaration, int times = 1) where T : TypeDeclarationSyntax => (T)typeDeclaration
            .WithoutTrivia().WithLeadingTrivia(TabulationTrivia.Repeat(times))
            .WithIdentifier(typeDeclaration.Identifier.WithoutTrivia().WithLeadingTrivia(WhitespaceTrivia).WithTrailingTrivia(NewLineTrivia))
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken).WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken).WithLeadingTrivia(TabulationTrivia.Repeat(times)).WithTrailingTrivia(NewLineTrivia));
    }
}
