using System.Text.RegularExpressions;

namespace BuildScripts;

[TaskName("Build Linux")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildLibraryTask))]
public sealed class BuildLinuxTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnLinux();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "build/";
        context.ReplaceTextInFiles("mojoshader/CMakeLists.txt", "ADD_LIBRARY(mojoshader", "ADD_LIBRARY(mojoshader SHARED ");
        context.ReplaceRegexInFiles("mojoshader/CMakeLists.txt", @"find_package\(SDL2\).+?ENDIF\(SDL2_FOUND\)", "", RegexOptions.Singleline);
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "../mojoshader/CMakeLists.txt -DCMAKE_OSX_ARCHITECTURES=\"x86_64;arm64\" -DPROFILE_SPIRV=OFF -DPROFILE_GLSPIRV=OFF" });
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build ." });
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "libmojoshader.so"), $"{context.ArtifactsDir}/libmojoshader.so");
    }
}
