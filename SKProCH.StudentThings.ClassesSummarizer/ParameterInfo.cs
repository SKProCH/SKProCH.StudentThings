namespace SKProCH.StudentThings.ClassesSummarizer;

record ParameterInfo(string Name, string Type, string? Summary) : FieldInfo(Name, Type);