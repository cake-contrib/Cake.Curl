#addin nuget:?package=Cake.Git&version=0.19.0

#load paths.cake

using System.Text.RegularExpressions;

public static bool LatestCommitHasVersionTag(this ICakeContext context)
{
    var latestTag = context.GitDescribe(Paths.RepositoryDirectory);
    var isVersionTag = Regex.IsMatch(latestTag, @"v[0-9]*");
    var noCommitsAfterLatestTag = !Regex.IsMatch(latestTag, @"\-[0-9]+\-");

    return isVersionTag && noCommitsAfterLatestTag;
}
