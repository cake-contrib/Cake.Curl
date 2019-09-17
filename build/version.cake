public static string GetVersionFromProjectFile(
    ICakeContext context,
    FilePath projectFile)
{
    return context.XmlPeek(
        projectFile,
        "/Project/PropertyGroup/Version/text()");
}
