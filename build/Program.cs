using System;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Common;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Restore;
using System.Threading;
using System.Diagnostics;
using Cake.Common.Tools.DotNet.Test;
using Cake.Common.Tools.DotNet.Build;
using static ProjectPaths;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext : FrostingContext
{
    public string Target { get; }
    public string BuildConfiguration { get; }
    public string SrcDirectoryPath { get; }
    public bool PushNuget { get; }
    public string NugetVersion { get; }
    public string NuGetPushToken { get; }
    public ProjectPaths ProjectPaths { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        Target = context.Argument("target", "Default");
        BuildConfiguration = LoadParameter(context, "configuration");
        SrcDirectoryPath = LoadParameter(context, "srcDirectoryPath");
        NugetVersion = LoadParameter(context, "nugetVersion");
        NuGetPushToken = LoadParameter(context, "nuGetPushToken");
        PushNuget = context.Argument<bool>("pushNuget", false);

        ProjectPaths = ProjectPaths.LoadFromContext(context, BuildConfiguration, SrcDirectoryPath, NugetVersion);
    }

    private string LoadParameter(ICakeContext context, string parameterName)
    {
        return context.Arguments.GetArgument(parameterName) ?? throw new Exception($"Missing parameter '{parameterName}'");
    }
}

[TaskName(nameof(OutputParametersTask))]
public sealed class OutputParametersTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Log.Information($"INFO: Current Working Directory: {context.Environment.WorkingDirectory}");

        context.Log.Information($"INFO: {nameof(context.BuildConfiguration)}: {context.BuildConfiguration}");
        context.Log.Information($"INFO: {nameof(context.SrcDirectoryPath)}: {context.SrcDirectoryPath}");
    }
}

[IsDependentOn(typeof(OutputParametersTask))]
[TaskName(nameof(BuildTask))]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        BuildDotnetApp(context, context.ProjectPaths.PathToSln);
        TestDotnetApp(context, context.ProjectPaths.UnitTestsProject);
        PublisNuget(context, context.ProjectPaths.ClientLibProject);
        PublisNuget(context, context.ProjectPaths.IndexMapperProject);
    }

    private void BuildDotnetApp(BuildContext context, string pathToSln)
    {
        context.DotNetRestore(pathToSln, new DotNetRestoreSettings { });

        context.DotNetBuild(pathToSln, new DotNetBuildSettings
        {
            NoRestore = true,
            Configuration = context.BuildConfiguration
        });
    }

    private void TestDotnetApp(BuildContext context, string pathToUnitTestProj)
    {
        var testSettings = new DotNetTestSettings()
        {
            Configuration = context.BuildConfiguration,
            NoBuild = true,
            ArgumentCustomization = (args) => args.Append("/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura --logger trx")
        };

        context.DotNetTest(pathToUnitTestProj, testSettings);
    }

    private void PublisNuget(BuildContext context, NugetProject nugetProj)
    {
        context.DotNetPack(nugetProj.CsProjFile, new Cake.Common.Tools.DotNet.Pack.DotNetPackSettings
        {
            IncludeSymbols = false,
            IncludeSource = true,
            NoBuild = true,
            Configuration = context.BuildConfiguration,
            OutputDirectory = nugetProj.OutDir,
            VersionSuffix = context.NugetVersion,
            ArgumentCustomization = (args) => args.Append($"-p:PackageVersion={context.NugetVersion}")
        });
    }
}

[IsDependentOn(typeof(BuildTask))]
[TaskName(nameof(NugetPushTask))]
public sealed class NugetPushTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (!context.PushNuget)
        {
            return;
        }

        context.DotNetNuGetPush(context.ProjectPaths.ClientLibProject.NugetFilePath, new Cake.Common.Tools.DotNet.NuGet.Push.DotNetNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = context.NuGetPushToken
        });

        context.DotNetNuGetPush(context.ProjectPaths.IndexMapperProject.NugetFilePath, new Cake.Common.Tools.DotNet.NuGet.Push.DotNetNuGetPushSettings
        {
            Source = "https://api.nuget.org/v3/index.json",
            ApiKey = context.NuGetPushToken
        });
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(NugetPushTask))]
public class DefaultTask : FrostingTask
{
}
