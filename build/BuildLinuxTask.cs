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
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "../mojoshader/CMakeLists.txt -DBUILD_SHARED_LIBS=ON" });
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build . --config release" });
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "libmojoshader.so"), $"{context.ArtifactsDir}/libmojoshader.so");
    }
}
