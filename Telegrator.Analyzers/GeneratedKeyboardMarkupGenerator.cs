using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Telegrator.Analyzers.RoslynExtensions;

namespace Telegrator.Analyzers
{
    [Generator(LanguageNames.CSharp)]
    public class GeneratedKeyboardMarkupGenerator : IIncrementalGenerator
    {
        // Return types
        private const string InlineReturnType = "InlineKeyboardMarkup";
        private const string ReplyReturnType = "ReplyKeyboardMarkup";

        // Attribute names
        private const string CallbackDataAttribute = "CallbackButton";
        private const string CallbackGameAttribute = "GameButton";
        private const string CopyTextAttribute = "CopyTextButton";
        private const string LoginRequestAttribute = "LoginRequestButton";
        private const string PayRequestAttribute = "PayRequestButton";
        private const string SwitchQueryAttribute = "SwitchQueryButton";
        private const string QueryChosenAttribute = "QueryChosenButton";
        private const string QueryCurrentAttribute = "QueryCurrentButton";
        private const string UrlRedirectAttribute = "UrlRedirectButton";
        private const string RequestChatAttribute = "RequestChatButton";
        private const string RequestContactAttribute = "RequestContactButton";
        private const string RequestLocationAttribute = "RequestLocationButton";
        private const string RequestPoolAttribute = "RequestPoolButton";
        private const string RequestUsersAttribute = "RequestUsersButton";
        private const string WebAppAttribute = "WebApp";

        // Markup lists
        private static readonly string[] InlineAttributes = [CallbackDataAttribute, CallbackGameAttribute, CopyTextAttribute, LoginRequestAttribute, PayRequestAttribute, UrlRedirectAttribute, WebAppAttribute, SwitchQueryAttribute, QueryChosenAttribute, QueryCurrentAttribute];
        private static readonly string[] ReplyAttributes = [RequestChatAttribute, RequestContactAttribute, RequestLocationAttribute, RequestPoolAttribute, RequestUsersAttribute, WebAppAttribute];
        
        // Usings
        private static readonly string[] DefaultUsings = ["Telegram.Bot.Types.ReplyMarkups"];

        // Markup layouts
        private static readonly Dictionary<string, MemberAccessExpressionSyntax> InlineKeyboardLayout = new Dictionary<string, MemberAccessExpressionSyntax>()
        {
            { CallbackDataAttribute, AccessExpression("InlineKeyboardButton", "WithCallbackData") },
            { CallbackGameAttribute, AccessExpression("InlineKeyboardButton", "WithCallbackGame") },
            { CopyTextAttribute, AccessExpression("InlineKeyboardButton", "WithCopyText") },
            { LoginRequestAttribute, AccessExpression("InlineKeyboardButton", "WithLoginUrl") },
            { PayRequestAttribute, AccessExpression("InlineKeyboardButton", "WithPay") },
            { SwitchQueryAttribute, AccessExpression("InlineKeyboardButton", "WithSwitchInlineQuery") },
            { QueryChosenAttribute, AccessExpression("InlineKeyboardButton", "WithSwitchInlineQueryChosenChat") },
            { QueryCurrentAttribute, AccessExpression("InlineKeyboardButton", "WithSwitchInlineQueryCurrentChat") },
            { UrlRedirectAttribute, AccessExpression("InlineKeyboardButton", "WithUrl") },
            { WebAppAttribute, AccessExpression("InlineKeyboardButton", "WithWebApp") },
        };

        private static readonly Dictionary<string, MemberAccessExpressionSyntax> ReplyKeyboardLayout = new Dictionary<string, MemberAccessExpressionSyntax>()
        {
            { RequestChatAttribute, AccessExpression("KeyboardButton", "WithRequestChat") },
            { RequestContactAttribute, AccessExpression("KeyboardButton", "WithRequestContact") },
            { RequestLocationAttribute, AccessExpression("KeyboardButton", "WithRequestLocation") },
            { RequestPoolAttribute, AccessExpression("KeyboardButton", "WithRequestPoll") },
            { RequestUsersAttribute, AccessExpression("KeyboardButton", "WithRequestUsers") },
            { WebAppAttribute, AccessExpression("KeyboardButton", "WithWebApp") }
        };

        // Markup map
        private static readonly Dictionary<string, Dictionary<string, MemberAccessExpressionSyntax>> LayoutNames = new Dictionary<string, Dictionary<string, MemberAccessExpressionSyntax>>()
        {
            { InlineReturnType, InlineKeyboardLayout },
            { ReplyReturnType, ReplyKeyboardLayout }
        };

        // Diagnostic descriptors
        private static readonly DiagnosticDescriptor WrongReturnType = new DiagnosticDescriptor("TG_1001", "Wrong return type", string.Empty, "Modelling", DiagnosticSeverity.Error, true);
        private static readonly DiagnosticDescriptor UnsupportedAttribute = new DiagnosticDescriptor("TG_1002", "Unsupported or invalid attribute", string.Empty, "Modelling", DiagnosticSeverity.Error, true);
        private static readonly DiagnosticDescriptor NotPartialMethod = new DiagnosticDescriptor("TG_1003", "Not a partial method", string.Empty, "Modelling", DiagnosticSeverity.Error, true);
        private static readonly DiagnosticDescriptor UseBodylessMethod = new DiagnosticDescriptor("TG_1004", "Use bodyless method", string.Empty, "Modelling", DiagnosticSeverity.Error, true);
        private static readonly DiagnosticDescriptor UseParametrlessMethod = new DiagnosticDescriptor("TG_1005", "Use parametrless method", string.Empty, "Modelling", DiagnosticSeverity.Error, true);

        // Trivias
        private static SyntaxTrivia TabulationTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, "\t");
        private static SyntaxTrivia WhitespaceTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " ");
        private static SyntaxTrivia NewLineTrivia => SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n");
        private static SyntaxToken Semicolon => SyntaxFactory.Token(SyntaxKind.SemicolonToken);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValueProvider<ImmutableArray<MethodDeclarationSyntax>> pipeline = context.SyntaxProvider
                .CreateSyntaxProvider(Provide, Transform)
                .Where(x => x != null)
                .Collect();

            context.RegisterSourceOutput(pipeline, Execute);
        }

        private static bool Provide(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (syntaxNode is not MethodDeclarationSyntax method)
                return false;

            if (!HasGenAttributes(method))
                return false;

            return true;
        }

        private static MethodDeclarationSyntax Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return (MethodDeclarationSyntax)context.Node;
        }

        private static void Execute(SourceProductionContext context, ImmutableArray<MethodDeclarationSyntax> methods)
        {
            List<GeneratedMarkupMethodModel> models = [];
            foreach (MethodDeclarationSyntax method in methods)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                try
                {
                    string methodName = method.Identifier.Text;
                    string returnType = method.ReturnType.ToString();
                    bool anyErrors = false;

                    if (!LayoutNames.TryGetValue(returnType, out var layout))
                    {
                        WrongReturnType.Report(context, method.ReturnType.GetLocation());
                        anyErrors = true;
                    }

                    if (!method.Modifiers.HasModifiers("partial"))
                    {
                        NotPartialMethod.Report(context, method.Identifier.GetLocation());
                        anyErrors = true;
                    }

                    if (method.ParameterList.Parameters.Any())
                    {
                        UseParametrlessMethod.Report(context, method.ParameterList.GetLocation());
                        anyErrors = true;
                    }

                    if (method.ExpressionBody != null)
                    {
                        UseBodylessMethod.Report(context, method.ExpressionBody.GetLocation());
                        anyErrors = true;
                    }

                    if (method.Body != null)
                    {
                        UseBodylessMethod.Report(context, method.Body.GetLocation());
                        anyErrors = true;
                    }

                    if (anyErrors)
                        return;

                    context.CancellationToken.ThrowIfCancellationRequested();
                    SeparatedSyntaxList<CollectionElementSyntax> vertical = new SeparatedSyntaxList<CollectionElementSyntax>();

                    foreach (AttributeListSyntax attributeList in method.AttributeLists)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();
                        SeparatedSyntaxList<CollectionElementSyntax> horizontal = new SeparatedSyntaxList<CollectionElementSyntax>();

                        foreach (AttributeSyntax attribute in attributeList.Attributes)
                        {
                            context.CancellationToken.ThrowIfCancellationRequested();

                            if (!layout.TryGetValue(attribute.Name.ToString(), out var accessSyntax))
                            {
                                UnsupportedAttribute.Report(context, attribute.Name.GetLocation());
                                return;
                            }

                            InvocationExpressionSyntax expression = SyntaxFactory.InvocationExpression(accessSyntax, ConvertArguments(attribute.ArgumentList));
                            horizontal = horizontal.Add(SyntaxFactory.ExpressionElement(expression));
                        }

                        ExpressionElementSyntax element = SyntaxFactory.ExpressionElement(SyntaxFactory.CollectionExpression(horizontal));
                        vertical = vertical.Add(element);
                    }

                    FieldDeclarationSyntax genField = GeneratedFieldDeclaration(methodName, method.ReturnType.WithoutTrivia(), SyntaxFactory.CollectionExpression(vertical));
                    MethodDeclarationSyntax genMethod = GeneratedMethodDeclaration(methodName, method.Modifiers, method.ReturnType, genField);
                    
                    models.Add(new GeneratedMarkupMethodModel(method, genField, genMethod));
                }
                catch (Exception ex)
                {
                    context.AddSource(method.Identifier.ToString(), ex.ToString());
                }
            }

            context.CancellationToken.ThrowIfCancellationRequested();
            CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit();
            SyntaxList<UsingDirectiveSyntax> usingDirectives = ParseUsings(DefaultUsings).ToSyntaxList();

            foreach (GeneratedMarkupMethodModel model in models)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    if (model.OriginalMethod.Parent is not ClassDeclarationSyntax)
                        throw new MissingMemberException();

                    NamespaceDeclarationSyntax genNamespace = GeneratedNamespaceDeclaration(model.OriginalMethod, [model.GeneratedField, model.GeneratedMethod]);
                    compilationUnit = compilationUnit.AddMembers(genNamespace);
                }
                catch (Exception ex)
                {
                    context.AddSource(model.OriginalMethod.Identifier.ToString(), ex.ToString());
                }
            }

            compilationUnit = compilationUnit.WithUsings(usingDirectives);
            context.AddSource("GeneratedKeyboards.g", compilationUnit.ToFullString());
        }

        private static MethodDeclarationSyntax GeneratedMethodDeclaration(string identifier, SyntaxTokenList modifiers, TypeSyntax returnType, FieldDeclarationSyntax field)
        {
            return SyntaxFactory.MethodDeclaration(returnType.WithTrailingTrivia(WhitespaceTrivia), identifier)
                .WithModifiers(modifiers)
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.IdentifierName(field.Declaration.Variables.ElementAt(0).Identifier)))
                .WithSemicolonToken(Semicolon);
        }

        private static FieldDeclarationSyntax GeneratedFieldDeclaration(string identifier, TypeSyntax returnType, CollectionExpressionSyntax collection)
        {
            ArgumentListSyntax arguments = SyntaxFactory.ArgumentList(SeparatedSyntaxList(SyntaxFactory.Argument(collection)));
            ObjectCreationExpressionSyntax objectCreation = SyntaxFactory.ObjectCreationExpression(returnType.WithLeadingTrivia(WhitespaceTrivia), arguments, null);

            VariableDeclaratorSyntax declarator = SyntaxFactory.VariableDeclarator(identifier + "_generatedMarkup")
                .WithInitializer(SyntaxFactory.EqualsValueClause(objectCreation));

            return SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(returnType.WithTrailingTrivia(WhitespaceTrivia)).AddVariables(declarator))
                .WithModifiers(Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword));
        }

        private static ArgumentListSyntax ConvertArguments(AttributeArgumentListSyntax? attributeArgs)
        {
            if (attributeArgs == null)
                return SyntaxFactory.ArgumentList();

            return SyntaxFactory.ArgumentList(SeparatedSyntaxList(attributeArgs.Arguments.Select(CastArgument)));
        }

        private static NamespaceDeclarationSyntax GeneratedNamespaceDeclaration(MethodDeclarationSyntax method, IEnumerable<MemberDeclarationSyntax> generatedMembers)
        {
            if (method.Parent is not ClassDeclarationSyntax containerClass)
                throw new MemberAccessException();

            int times = method.CountParentTree();
            ClassDeclarationSyntax generatedContainerClass = SyntaxFactory.ClassDeclaration(containerClass.Identifier)
                .WithMembers(new SyntaxList<MemberDeclarationSyntax>(generatedMembers.Select(member => member.DecorateMember(times + 1))))
                .WithModifiers(containerClass.Modifiers.Decorate())
                .DecorateType(times);

            MemberDeclarationSyntax generated = generatedContainerClass;
            MemberDeclarationSyntax inspecting = containerClass;

            while (inspecting.Parent != null)
            {
                times -= 1;
                if (inspecting.Parent is not MemberDeclarationSyntax inspectingMember)
                    break;

                inspecting = inspectingMember;
                switch (inspectingMember)
                {
                    case ClassDeclarationSyntax classDeclaration:
                        {
                            generated = SyntaxFactory.ClassDeclaration(classDeclaration.Identifier)
                                .WithMembers([generated])
                                .WithModifiers(classDeclaration.Modifiers.Decorate())
                                .DecorateType(times);

                            break;
                        }

                    case StructDeclarationSyntax structDeclaration:
                        {
                            generated = SyntaxFactory.StructDeclaration(structDeclaration.Identifier)
                                .WithMembers([generated])
                                .WithModifiers(structDeclaration.Modifiers.Decorate())
                                .DecorateType(times);

                            break;
                        }

                    case NamespaceDeclarationSyntax namespaceDeclaration:
                        {
                            //foundNamespaceDeclaration = namespaceDeclaration;
                            return SyntaxFactory.NamespaceDeclaration(namespaceDeclaration.Name)
                                .WithMembers([generated]).Decorate();
                        }
                }
            }

            throw new AncestorNotFoundException();
        }

        private static ArgumentSyntax CastArgument(AttributeArgumentSyntax argument)
            => SyntaxFactory.Argument(argument.Expression).WithNameColon(argument.NameColon);

        private static SyntaxTokenList Modifiers(params SyntaxKind[] kinds)
            => new SyntaxTokenList(kinds.Select(SyntaxFactory.Token).Select(mod => mod.WithTrailingTrivia(WhitespaceTrivia)));

        private static IEnumerable<UsingDirectiveSyntax> ParseUsings(params string[] names) => names
            .Select(name => SyntaxFactory.IdentifierName(name).WithLeadingTrivia(WhitespaceTrivia))
            .Select(name => SyntaxFactory.UsingDirective(name).WithTrailingTrivia(NewLineTrivia));

        private static bool HasGenAttributes(MethodDeclarationSyntax method) => method.AttributeLists.SelectMany(x => x.Attributes)
            .Select(x => x.Name.ToString()).Intersect(InlineAttributes.Concat(ReplyAttributes)).Any();

        private static SeparatedSyntaxList<T> SeparatedSyntaxList<T>(params IEnumerable<T> elements) where T : SyntaxNode
            => new SeparatedSyntaxList<T>().AddRange(elements);

        private static MemberAccessExpressionSyntax AccessExpression(string className, string methodName)
            => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(className), SyntaxFactory.IdentifierName(methodName));

        private record class GeneratedMarkupMethodModel(MethodDeclarationSyntax OriginalMethod, FieldDeclarationSyntax GeneratedField, MethodDeclarationSyntax GeneratedMethod);
    }
}
