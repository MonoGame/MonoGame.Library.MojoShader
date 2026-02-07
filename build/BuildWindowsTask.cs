using System.Text.RegularExpressions;

namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildLibraryTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        BuildForArchitecture(context, "x64", "x64");
        BuildForArchitecture(context, "ARM64", "arm64");
    }

    private void BuildForArchitecture(BuildContext context, string arch, string rid)
    {
        var buildWorkingDir = $"mojoshaderbuild-{rid}/";
        context.CreateDirectory(buildWorkingDir);
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = $"-A {arch} ../mojoshader/CMakeLists.txt" });

        // Fix generated projects using the same obj folder
        var dirProps = @"
        <Project>
            <PropertyGroup>
                    <BaseIntermediateOutputPath>$(MSBuildProjectDirectory)\obj\$(MSBuildProjectName)</BaseIntermediateOutputPath>
            </PropertyGroup>
        </Project>
        ";
        context.FileWriteText(System.IO.Path.Combine(buildWorkingDir, "Directory.Build.props"), dirProps);

        // Statically link VC runtime
        context.ReplaceTextInFiles(System.IO.Path.Combine(buildWorkingDir, "mojoshader.vcxproj"), "MultiThreadedDLL", "MultiThreaded");

        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build . --config release" });
        
        var outputDir = $"{context.ArtifactsDir}/win-{rid}";
        context.CreateDirectory(outputDir);
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "Release", "mojoshader.dll"), $"{outputDir}/mojoshader.dll");
    }
}
