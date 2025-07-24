using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Telegrator.Analyzers
{
    [Generator(LanguageNames.CSharp)]
    public class DeveloperHelperAnalyzer : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValueProvider<ImmutableArray<HandlerDeclarationModel>> pipeline = context.SyntaxProvider
                .CreateSyntaxProvider(Provide, Transform)
                .Where(handler => handler != null)
                .Collect();

            context.RegisterSourceOutput(pipeline, Execute);
        }

        private static bool Provide(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (syntaxNode is not ClassDeclarationSyntax classSyntax)
                return false;

            if (classSyntax.BaseList?.Types.Count == 0 && classSyntax.AttributeLists.SelectMany(list => list.Attributes).Count() == 0)
                return false;

            return true;
        }

        private static HandlerDeclarationModel Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            ClassDeclarationSyntax classSyntax = (ClassDeclarationSyntax)context.Node;
            IEnumerable<AttributeSyntax> attributes = classSyntax.GetHandlerAttributes();
            BaseTypeSyntax? baseType = classSyntax.GetHandlerBaseClass();

            if (baseType == null && !attributes.Any())
                return null!;

            return new HandlerDeclarationModel(classSyntax, attributes, baseType);
        }

        private static void Execute(SourceProductionContext context, ImmutableArray<HandlerDeclarationModel> handlers)
        {
            StringBuilder sourceBuilder = new StringBuilder();
            List<string> usingDirectives = [];

            sourceBuilder
                .AppendTabs(0).Append("namespace Telegrator.Analyzers").AppendLine()
                .AppendTabs(0).Append("{").AppendLine()
                .AppendTabs(1).Append("public static partial class AnalyzerExport").AppendLine()
                .AppendTabs(1).Append("{").AppendLine();

            foreach (HandlerDeclarationModel handler in handlers)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                try
                {
                    usingDirectives.UnionAdd(handler.ClassDeclaration.FindCompilationUnitSyntax().Usings.Select(use => use.ToString()));
                    ParseHandlerDeclaration(context, sourceBuilder, handler, context.CancellationToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    sourceBuilder.AppendLine()
                        .Append("// Failed to parse ").Append(handler.ClassDeclaration.Identifier.ToString()).AppendLine()
                        .Append(ex).AppendLine();
                }
            }

            sourceBuilder.AppendLine("\t}\n}");
            sourceBuilder.Insert(0, string.Join("\n", usingDirectives.OrderBy(use => use)) + "\n\n");
            //context.AddSource("DeveloperHelperAnalyzer.cs", sourceBuilder.ToString());
        }

        private static void ParseHandlerDeclaration(SourceProductionContext context, StringBuilder sourceBuilder, HandlerDeclarationModel handler, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sourceBuilder.Append("//").Append(handler.ClassDeclaration.Identifier.ToString()).AppendLine();
            //context.ReportDiagnostic(DiagnosticsHelper.Test.Create(handler.ClassDeclaration.Identifier.GetLocation()));
        }
    }

    internal static class DeveloperHelperAnalyzerExtensions
    {
        private static readonly string[] AttributeNames =
        [
            "AnyUpdateHandlerAttribute",
            "CallbackQueryHandlerAttribute",
            "CommandHandlerAttribute",
            "WelcomeHandlerAttribute",
            "MessageHandlerAttribute"
        ];

        private static readonly string[] HandlersNames =
        [
            "AnyUpdateHandler",
            "CallbackQueryHandler",
            "CommandHandler",
            "WelcomeHandler",
            "MessageHandler"
        ];

        public static IEnumerable<AttributeSyntax> GetHandlerAttributes(this ClassDeclarationSyntax classSyntax)
        {
            IEnumerable<AttributeSyntax> attributes = classSyntax.AttributeLists.SelectMany(list => list.Attributes);
            return attributes.IntersectBy(AttributeNames, attr => attr.Name.ToString());
        }

        public static BaseTypeSyntax? GetHandlerBaseClass(this ClassDeclarationSyntax classSyntax)
        {
            return classSyntax.BaseList?.Types.FirstOrDefault(type => HandlersNames.Contains(type.ToString()));
        }
    }
}
