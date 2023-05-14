using Microsoft.CodeAnalysis;

namespace SKProCH.StudentThings.ClassesSummarizer;

public record FieldInfo(string Name, string Type) {
    public static FieldInfo FromField(IFieldSymbol fieldSymbol)
        => new(fieldSymbol.Name, fieldSymbol.Type.Name);
    public static FieldInfo FromParameter(IParameterSymbol parameterSymbol)
        => new(parameterSymbol.Name, parameterSymbol.Type.Name);
}