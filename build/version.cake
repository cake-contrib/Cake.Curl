public static string GetVersionFromProjectFile(
    ICakeContext context,
    FilePath projectFile)
{
    return context.XmlPeek(
        projectFile,
        "/Project/PropertyGroup/Version/text()");
}

public static void SetVersionToProjectFile(
    ICakeContext context,
    FilePath projectFile,
    string version)
{
    context.XmlPoke(
        projectFile,
        "/Project/PropertyGroup/Version",
        version);
}
