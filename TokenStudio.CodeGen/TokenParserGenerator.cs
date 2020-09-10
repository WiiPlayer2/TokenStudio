using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TokenStudio.CodeGen
{
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
                var parserClassDeclaration = ClassDeclaration($"{enumDeclarationSyntax.Identifier.ValueText}Parser")
                    .WithBaseList(
                        BaseList(
                            SingletonSeparatedList<BaseTypeSyntax>(
                                SimpleBaseType(
                                    QualifiedName(
                                        QualifiedName(
                                            IdentifierName("TokenStudio"),
                                            IdentifierName("Core")),
                                        GenericName(
                                            Identifier("TokenParser"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList(
                                                SingletonSeparatedList<TypeSyntax>(
                                                    IdentifierName(enumDeclarationSyntax.Identifier)))))))));

                results = results.Add(parserClassDeclaration);
            }
            return Task.FromResult(results);
        }
    }
}