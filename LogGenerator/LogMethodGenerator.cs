using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace LogGenerator
{
    [Generator]
    public class LogMethodGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;

            foreach (var tree in syntaxTrees)
            {
                var root = compilation.GetSemanticModel(tree).SyntaxTree.GetRoot();
                var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

                foreach (var method in methods)
                {
                    var hasLogAttribute = method.AttributeLists.SelectMany(al => al.Attributes)
                       .Any(a => a.Name.ToString() == nameof(TraceLogger));

                    if (!hasLogAttribute) continue;

                    var originalCode = method.GetText().ToString();
                    var modifiedCode = new StringBuilder(originalCode);

                    // Append code at the beginning and end of the method
                    modifiedCode.Insert(method.Body.SpanStart, @"Log.Information(""Before"");");
                    //modifiedCode.Insert(method.Body., "Console.WriteLine(\"After\");");

                    context.AddSource($"{method.Identifier}", SourceText.From(modifiedCode.ToString(), Encoding.UTF8));
                }
            }
        }
    }
}
