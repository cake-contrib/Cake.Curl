#addin nuget:?package=Newtonsoft.Json&version=9.0.1

public static string GetVersionFromProjectFile(
    DirectoryPath projectDirectory)
{
    var projectFilePath = projectDirectory.GetFilePath("project.json").FullPath;
    var content = System.IO.File.ReadAllText(projectFilePath);
    var project = Newtonsoft.Json.Linq.JObject.Parse(content);

    return (string)project["version"];
}

public static void SetVersionToProjectFile(
    string version,
    DirectoryPath projectDirectory)
{
    var projectFilePath = projectDirectory.GetFilePath("project.json").FullPath;
    var content = System.IO.File.ReadAllText(projectFilePath);
    var project = Newtonsoft.Json.Linq.JObject.Parse(content);

    project["version"] = version;
    System.IO.File.WriteAllText(projectFilePath, project.ToString());
}
