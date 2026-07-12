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
        var buildWorkingDir = "mojoshaderbuild/";
        context.CreateDirectory(buildWorkingDir);
        context.StartProcessWithDocker("cmake", workingDirectory: buildWorkingDir, args: "../mojoshader/CMakeLists.txt");
        context.StartProcessWithDocker("cmake", workingDirectory: buildWorkingDir, args: "--build . --config release");
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "libmojoshader.so"), $"{context.ArtifactsDir}/libmojoshader.so");
    }
}
