using System;
using System.Linq;
using System.Xml.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Git;
using Nuke.Common.CI.GitHubActions;
using Serilog;
using GlobExpressions;
using System.IO;
using System.Text;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.CreateNuget);

    [Parameter(Name = "NUGET_API_KEY")] readonly string NuGetApiKey;
    [Parameter(Name = "LIB_VERSION")] readonly string Version;
    [GitRepository] readonly GitRepository GitRepository;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    // ───── Путь до проекта и папок ─────────────────────
    AbsolutePath Root => RootDirectory;
    AbsolutePath OutputDirectory => Root / "artifacts";
    AbsolutePath LibCoreFile => Root / "src" / "Navodkin.Erraruga.Core" / "Navodkin.Erraruga.Core.csproj";
    AbsolutePath TestProject => Root / "tests" / "Navodkin.Erraruga.Core.Tests" / "Navodkin.Erraruga.Core.Tests.csproj";

    // ───── Шаги сборки ─────────────────────────────────

    string GetVersion()
    {
        if (IsLocalBuild)
        {
            // Для локальной сборки используем версию из common.props
            var propsFile = Root / "common.props";
            var doc = XDocument.Load(propsFile);
            var version = doc
                .Descendants("Version")
                .FirstOrDefault()
                ?.Value
                ?.Trim();

            if (string.IsNullOrEmpty(version))
                throw new Exception("Version not found in common.props");

            return version;
        }

        if (string.IsNullOrEmpty(Version))
            throw new Exception("VERSION parameter is required for server builds");

        // Удаляем префикс "v" если он есть
        var cleanVersion = Version.TrimStart('v');
        Log.Information("Original version: {OriginalVersion}, Clean version: {CleanVersion}", Version, cleanVersion);
        
        return cleanVersion;
    }

    Target Clean => _ => _
        .Executes(() =>
        {
            Log.Information("////////////////////////////////", OutputDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(LibCoreFile));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(LibCoreFile)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target RunTests => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(TestProject)
                .SetConfiguration(Configuration));
        });

    Target CreateNuget => _ => _
        .DependsOn(RunTests)
        .Executes(() =>
        {
            var version = GetVersion();

            Log.Information("Using version: {Version}", version);

            DotNetPack(s => s
                .SetProject(LibCoreFile)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(OutputDirectory)
                .EnableNoBuild()
                .SetVersion(version));
        });



    Target GetReleaseContent => _ => _
        .DependsOn(CreateNuget)
        .Executes(() =>
        {
            Log.Information("Using version: {Version}", Version);

            var changelogFile = Root / "CHANGELOG.md";

            var changelogContent = File.ReadAllText(changelogFile, Encoding.UTF8);

            var entry = ChangelogParser.ParseVersion(changelogContent, GetVersion());

            if (entry == null)
            {
                throw new Exception($"Version {Version} not found in changelog.");
            }

            var githubOutput = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");

            if (!string.IsNullOrEmpty(githubOutput))
            {
                File.AppendAllText(githubOutput, $"RELEASE_CONTENT<<EOF{Environment.NewLine}{entry.ToString()}{Environment.NewLine}EOF{Environment.NewLine}");
            }
            else
            {
                throw new Exception("Cant find content");
            }
        });

    Target TestParse => _ => _
        .Executes(() =>
        {
            var changelogFile = Root / "CHANGELOG.md";

            var changelogContent = File.ReadAllText(changelogFile, Encoding.UTF8);

            var entry = ChangelogParser.ParseVersion(changelogContent, "5.0.0");

           
        });

    Target ReleaseBaby => _ => _
        .DependsOn(GetReleaseContent)
        .Executes(() =>
        {
            Log.Information("Okay :D run this shit!");
        });

    //Target PublishNuget => _ => _
    //    .DependsOn(CreateNuget)
    //    .Requires(() => NuGetApiKey)
    //    .Executes(() =>
    //    {
    //        var packages = Glob.Files(OutputDirectory, "*.nupkg");
    //        Log.Information("Found packages: {Packages}", string.Join(", ", packages));
            
    //        foreach (var package in packages)
    //        {
    //            var fullPath = OutputDirectory / package;
    //            Log.Information("Publishing package: {PackagePath}", fullPath);
                
    //            DotNetNuGetPush(s => s
    //                .SetTargetPath(fullPath)
    //                .SetSource("https://api.nuget.org/v3/index.json")
    //                .SetApiKey(NuGetApiKey));
    //        }
    //    });
}
