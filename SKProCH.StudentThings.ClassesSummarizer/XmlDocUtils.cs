using System;
using System.IO;
using System.Xml;
using Microsoft.CodeAnalysis;

namespace SKProCH.StudentThings.ClassesSummarizer;

public static class XmlDocUtils {
    public static string? GetSummary(string xmlDoc) {
        try {
            using var stringReader = new StringReader("");
            using var reader = XmlReader.Create(stringReader);
            var summaryExists = reader.ReadToFollowing("summary");
            return !summaryExists ? null : reader.ReadElementContentAsString();
        }
        catch (Exception) {
            return null;
        }
    }

    public static string? GetSummaryFromSymbol(ISymbol symbol) {
        var documentationCommentXml = symbol.GetDocumentationCommentXml();
        return documentationCommentXml != null ? GetSummary(documentationCommentXml) : null;
    }
}