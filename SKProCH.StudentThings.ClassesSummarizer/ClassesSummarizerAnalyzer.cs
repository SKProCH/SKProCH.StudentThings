using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SKProCH.StudentThings.ClassesSummarizer.Formatters;
#pragma warning disable RS1035

namespace SKProCH.StudentThings.ClassesSummarizer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ClassesSummarizerAnalyzer : DiagnosticAnalyzer {
    public static readonly ImmutableArray<IReportFormatter> Formatters = ImmutableArray.Create<IReportFormatter>(new JsonReportFormatter(), new PlaintextReportFormatter());
    /// <inheritdoc />
    public override void Initialize(AnalysisContext context) {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private void OnCompilationStart(CompilationStartAnalysisContext compilationStartContext) {
        var testDatas = new List<ClassData>();
        compilationStartContext.RegisterSymbolAction(context => testDatas.Add(GetClassData(context)), SymbolKind.NamedType);
        compilationStartContext.RegisterCompilationEndAction(context => ReportResults(context, testDatas));
    }

    private void ReportResults(CompilationAnalysisContext analysisContext, IReadOnlyCollection<ClassData> classDatas) {
        if (analysisContext.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir)) {
            var path = Path.Combine(projectDir, "bin");
            Directory.CreateDirectory(path);

            ReportResults(classDatas, (reportType, reportText) => {
                File.WriteAllText(Path.Combine(path, "ClassesSummarizer." + reportType), reportText);
            });
            analysisContext.ReportDiagnostic(Diagnostic.Create(SummarySavedDiagnostic, null, path));
            return;
        }

        ReportResults(classDatas, (reportType, reportText) => {
            analysisContext.ReportDiagnostic(Diagnostic.Create(SummaryGeneratedDiagnostic, null, reportType, reportText));
        });
    }

    public void ReportResults(IReadOnlyCollection<ClassData> classDatas, Action<string, string> doReport) {
        foreach (var reportFormatter in Formatters) {
            var reportText = reportFormatter.GetReport(classDatas);
            doReport(reportFormatter.FormatName, reportText);
        }
    }

    private ClassData GetClassData(SymbolAnalysisContext analysisContext) {
        var symbol = (INamedTypeSymbol)analysisContext.Symbol;
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
            .Where(methodSymbol => methodSymbol.MethodKind == MethodKind.DeclareMethod);

        var publicMethods = methods
            .Where(methodSymbol => methodSymbol.DeclaredAccessibility == Accessibility.Public)
            .Select(MethodInfo.FromMethod);

        var privateMethods = methods
            .Where(methodSymbol => methodSymbol.DeclaredAccessibility != Accessibility.Public)
            .Select(MethodInfo.FromMethod);

        return new ClassData(symbol.Name, symbol.BaseType?.Name, interfaceNames, constants, fields, publicMethods, privateMethods);
    }


    private const string Category = "Usage";
    private static readonly DiagnosticDescriptor SummarySavedDiagnostic = new("DS001",
        "Classes summary saved",
        "Classes summary saved to: {0}",
        Category, DiagnosticSeverity.Info, isEnabledByDefault: true);
    private static readonly DiagnosticDescriptor SummaryGeneratedDiagnostic = new("DS002",
        "Classes summary generated",
        "Classes summary generated, be output directory can't be located. Here is the output in {0} format: {1}",
        Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(SummarySavedDiagnostic, SummaryGeneratedDiagnostic);
}