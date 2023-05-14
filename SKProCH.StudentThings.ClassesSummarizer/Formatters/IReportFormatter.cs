using System.Collections.Generic;

namespace SKProCH.StudentThings.ClassesSummarizer.Formatters; 

public interface IReportFormatter {
    string FormatName { get; }
    string GetReport(IReadOnlyCollection<ClassData> classDatas);
}