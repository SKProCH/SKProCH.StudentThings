using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SKProCH.StudentThings.ClassesSummarizer;

public record MethodInfo(string Name, string? Summary, string ReturnType, IEnumerable<FieldInfo> Parameters) {
    public static MethodInfo FromMethod(IMethodSymbol methodSymbol) {
        var summary = XmlDocUtils.GetSummaryFromSymbol(methodSymbol);
        var returnTypeName = methodSymbol.ReturnsVoid ? "void" : methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var parameters = methodSymbol.Parameters.Select(FieldInfo.FromParameter);
        return new MethodInfo(methodSymbol.Name, summary, returnTypeName, parameters);
    }
}