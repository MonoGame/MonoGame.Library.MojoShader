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
        context.StartProcessWithDocker("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "../mojoshader/CMakeLists.txt" });
        context.StartProcessWithDocker("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build . --config release" });
        var artifactPath = $"{context.ArtifactsDir}/libmojoshader.so";
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "libmojoshader.so"), artifactPath);

        var stripArguments = new ProcessArgumentBuilder();
        stripArguments.Append("--strip-unneeded");
        stripArguments.AppendQuoted(artifactPath);
        context.StartProcessWithDocker("strip", new ProcessSettings { WorkingDirectory = "", Arguments = stripArguments });
    }
}
