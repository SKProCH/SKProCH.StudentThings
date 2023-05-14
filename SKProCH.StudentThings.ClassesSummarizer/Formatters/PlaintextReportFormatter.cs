using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKProCH.StudentThings.ClassesSummarizer.Formatters;

class PlaintextReportFormatter : IReportFormatter {
    /// <inheritdoc />
    public string FormatName { get; } = "plaintext";
    /// <inheritdoc />
    public string GetReport(IReadOnlyCollection<ClassData> classDatas) {
        var stringBuilder = new StringBuilder();
        foreach (var data in classDatas) {
            stringBuilder.AppendLine($"Class description {data.Name}");
            if (data.BaseTypeName != null) {
                stringBuilder.AppendLine($"	Base class {data.BaseTypeName}");
            }
            if (data.ImplementedInterfaces.Any()) {
                stringBuilder.AppendLine("	Interfaces:");
                foreach (var @interface in data.ImplementedInterfaces) {
                    stringBuilder.AppendLine($"			{@interface}");
                }
            }
            if (data.Constants.Any()) {
                stringBuilder.AppendLine("	Constants:");
                foreach (var fieldInfo in data.Constants) {
                    stringBuilder.AppendLine($"			{fieldInfo.Type} {fieldInfo.Name}");
                }
            }
            if (data.Fields.Any()) {
                stringBuilder.AppendLine("	Fields:");
                foreach (var fieldInfo in data.Fields) {
                    stringBuilder.AppendLine($"			{fieldInfo.Type} {fieldInfo.Name}");
                }
            }
            if (data.PublicMethods.Any()) {
                stringBuilder.AppendLine("	Public methods:");
                foreach (var methodInfo in data.PublicMethods) {
                    stringBuilder.AppendLine($"		{methodInfo.Name}");
                    stringBuilder.AppendLine($"			Returns: {methodInfo.ReturnType}");
                    if (methodInfo.Summary != null) {
                        stringBuilder.AppendLine($"			Summary: {methodInfo.Summary}");
                    }
                    if (methodInfo.Parameters.Any()) {
                        stringBuilder.AppendLine($"			Parameters:");
                        foreach (var parameter in methodInfo.Parameters) {
                            stringBuilder.AppendLine($"				{parameter.Type} {parameter.Name}");
                        }
                    }
                }
            }
        }
        return stringBuilder.ToString();
    }
}