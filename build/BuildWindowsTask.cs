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
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "../mojoshader/CMakeLists.txt -DBUILD_SHARED_LIBS=ON -DDEPTH_CLIPPING=ON -DPROFILE_GLSLES3=ON" });

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
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "Release", "mojoshader.dll"), $"{context.ArtifactsDir}/mojoshader.dll");
    }
}
