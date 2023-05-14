using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SKProCH.StudentThings.ClassesSummarizer;

public record ClassData(string Name, string? BaseTypeName, IEnumerable<string> ImplementedInterfaces,
                        IEnumerable<FieldInfo> Constants, IEnumerable<FieldInfo> Fields,
                        IEnumerable<MethodInfo> PublicMethods, IEnumerable<MethodInfo> PrivateMethods) {
    private static readonly ImmutableArray<MethodKind> AllowedMethodKinds = 
        ImmutableArray.Create(MethodKind.DeclareMethod, MethodKind.ExplicitInterfaceImplementation, MethodKind.Ordinary, MethodKind.ReducedExtension);
    
    public static ClassData FromClass(INamedTypeSymbol symbol) {
        var baseType = symbol.BaseType?.Name == "Object" && symbol.BaseType?.ContainingNamespace.Name == "System" 
            ? null 
            : symbol.BaseType?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var interfaceNames = symbol.Interfaces.Select(typeSymbol => typeSymbol.Name);
        var members = symbol.GetMembers();

        var constants = members
            .OfType<IFieldSymbol>()
            .Where(fieldSymbol => fieldSymbol.IsConst)
            .Select(FieldInfo.FromField);

        var fields = members
            .OfType<IFieldSymbol>()
            .Where(fieldSymbol => !fieldSymbol.IsImplicitlyDeclared)
            .Where(fieldSymbol => !fieldSymbol.IsConst)
            .Select(FieldInfo.FromField);

        var methods = members
            .OfType<IMethodSymbol>()
            .Where(methodSymbol => !methodSymbol.IsImplicitlyDeclared)
            .Where(methodSymbol => AllowedMethodKinds.Contains(methodSymbol.MethodKind))
            .ToList();

        var publicMethods = methods
            .Where(methodSymbol => methodSymbol.DeclaredAccessibility == Accessibility.Public)
            .Select(MethodInfo.FromMethod);

        var privateMethods = methods
            .Where(methodSymbol => methodSymbol.DeclaredAccessibility != Accessibility.Public)
            .Select(MethodInfo.FromMethod);

        return new ClassData(symbol.Name, baseType, interfaceNames, constants, fields, publicMethods, privateMethods);
    }
}