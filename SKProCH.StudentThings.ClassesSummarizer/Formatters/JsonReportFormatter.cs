using System.Collections.Generic;
using System.Text.Json;

namespace SKProCH.StudentThings.ClassesSummarizer.Formatters;

class JsonReportFormatter : IReportFormatter {
    /// <inheritdoc />
    public string FormatName { get; } = "json";
    /// <inheritdoc />
    public string GetReport(IReadOnlyCollection<ClassData> classDatas) {
        return JsonSerializer.Serialize(classDatas);
    }
}