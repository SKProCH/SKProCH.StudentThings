using Microsoft.CodeAnalysis;

namespace SKProCH.StudentThings.ClassesSummarizer;

public record FieldInfo(string Name, string Type) {
    public static FieldInfo FromField(IFieldSymbol fieldSymbol)
        => new(fieldSymbol.Name, fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
    public static FieldInfo FromParameter(IParameterSymbol parameterSymbol)
        => new(parameterSymbol.Name, parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
}