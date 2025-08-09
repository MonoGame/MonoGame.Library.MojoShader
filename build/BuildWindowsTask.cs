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
        var buildWorkingDir = "mojoshaderbuild/";
        context.CreateDirectory(buildWorkingDir);
        context.ReplaceTextInFiles("mojoshader/CMakeLists.txt", "ADD_LIBRARY(mojoshader", "ADD_LIBRARY(mojoshader SHARED ");
        context.ReplaceRegexInFiles("mojoshader/CMakeLists.txt", @"find_package\(SDL2\).+?ENDIF\(SDL2_FOUND\)", "", RegexOptions.Singleline);
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "../mojoshader/CMakeLists.txt -DPROFILE_SPIRV=OFF -DPROFILE_GLSPIRV=OFF" });

        // Fix generated projects using the same obj folder
        var dirProps = @"
        <Project>
            <PropertyGroup>
                    <BaseIntermediateOutputPath>$(MSBuildProjectDirectory)\obj\$(MSBuildProjectName)</BaseIntermediateOutputPath>
            </PropertyGroup>
        </Project>
        ";
        context.FileWriteText(System.IO.Path.Combine(buildWorkingDir, "Directory.Build.props"), dirProps);

        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build ." });
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "Release", "mojoshader.dll"), $"{context.ArtifactsDir}/mojoshader.dll");
    }
}
