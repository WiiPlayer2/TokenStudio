using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TokenStudio.CodeGen
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;

    public class TokenParserGenerator : ICodeGenerator
    {
        public TokenParserGenerator(AttributeData attributeData)
        {
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var results = List<MemberDeclarationSyntax>();
            if (context.ProcessingNode is EnumDeclarationSyntax enumDeclarationSyntax)
            {
                var className = $"{enumDeclarationSyntax.Identifier.ValueText}Parser";
                var memberPatterns = enumDeclarationSyntax.Members
                    .Select(
                        member => (member,
                            ModelExtensions.GetDeclaredSymbol(context.SemanticModel, member).GetAttributes()
                                .FirstOrDefault(/*attributeData => attributeData.AttributeClass.Name == "Pattern"*/)?.ConstructorArguments[0].Value as string))
                    .Where(pair => pair.Item2 != null);

                var parserClassDeclaration = ClassDeclaration(className)
                    .WithBaseList(
                        BaseList(
                            SingletonSeparatedList<BaseTypeSyntax>(
                                SimpleBaseType(
                                    QualifiedName(
                                        QualifiedName(IdentifierName("TokenStudio"), IdentifierName("Core")),
                                        GenericName(Identifier("TokenParser")).WithTypeArgumentList(
                                            TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(enumDeclarationSyntax.Identifier)))))))))
                    .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            ConstructorDeclaration(Identifier(className)).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword))).WithBody(
                                Block(memberPatterns.Select(pair => GetAddPatternCallForMember(enumDeclarationSyntax, pair.member, pair.Item2)).ToArray()))));

                results = results.Add(parserClassDeclaration);
            }
            return Task.FromResult(results);
        }

        ExpressionStatementSyntax GetAddPatternCallForMember(EnumDeclarationSyntax enumDeclarationSyntax, EnumMemberDeclarationSyntax memberDeclarationSyntax, string pattern)
        {
            return ExpressionStatement(
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName("AddPattern")))
                    .WithArgumentList(
                        ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(enumDeclarationSyntax.Identifier),
                                            IdentifierName(memberDeclarationSyntax.Identifier))),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(pattern)))
                                }))));
        }
    }
}