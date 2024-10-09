using Cake.Common;
using Cake.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ProjectPaths;

public record ProjectPaths(
    string PathToSln,
    string UnitTestsProject,
    NugetProject ClientLibProject,
    NugetProject IndexMapperProject)
{
    public static ProjectPaths LoadFromContext(ICakeContext context, string buildConfiguration, string srcDirectory, string nugetVersion)
    {
        var pathToSln = srcDirectory + $"/CosmosDbIndexConfigurator.sln";

        var unitTestsProject = $"{srcDirectory}/UnitTests/UnitTests.csproj";

        var clientLibProject = NugetProject.LoadFromContext(buildConfiguration, srcDirectory, nugetVersion, "ClientLib");
        var indexMapperProject = NugetProject.LoadFromContext(buildConfiguration, srcDirectory, nugetVersion, "IndexMapper");

        return new ProjectPaths(
            pathToSln,
            unitTestsProject,
            clientLibProject,
            indexMapperProject);
    }

    public record NugetProject(
        string CsProjFile,
        string OutDir,
        string NugetFilePath)
    {
        public static NugetProject LoadFromContext(string buildConfiguration, string srcDirectory, string nugetVersion, string projectName)
        {
            var projectDir = $"{srcDirectory}/{projectName}";
            var outDir = $"{projectDir}/bin/{buildConfiguration}/cake-build-output";

            return new NugetProject(
                CsProjFile: $"{projectDir}/{projectName}.csproj",
                OutDir: outDir,
                NugetFilePath: $"{outDir}/*{nugetVersion}.nupkg"
                );
        }
    }
};
