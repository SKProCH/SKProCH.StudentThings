using System.Collections.Generic;

namespace SKProCH.StudentThings.ClassesSummarizer;

public record ClassData(string Name, string? BaseTypeName, IEnumerable<string> ImplementedInterfaces,
                       IEnumerable<FieldInfo> Constants, IEnumerable<FieldInfo> Fields,
                       IEnumerable<MethodInfo> PublicMethods, IEnumerable<MethodInfo> PrivateMethods);