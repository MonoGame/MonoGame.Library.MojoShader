using Cake.Common.Tools.MSBuild;

namespace BuildScripts;

[TaskName("Build Windows")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildLibraryTask))]
public sealed class BuildWindowsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnWindows();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "build/";
        context.ReplaceTextInFiles("mojoshader/CMakeLists.txt", "ADD_LIBRARY(mojoshader", "ADD_LIBRARY(mojoshader SHARED ");
        context.ReplaceRegexInFiles("mojoshader/CMakeLists.txt", @"find_package\(SDL2\).+?ENDIF\(SDL2_FOUND\)", "");
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "../mojoshader/CMakeLists.txt -DPROFILE_SPIRV=OFF -DPROFILE_GLSPIRV=OFF" });
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build ." });
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "mojoshader.dll"), $"{context.ArtifactsDir}/mojoshader.dll");
    }
}
