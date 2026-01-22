using System.Text.RegularExpressions;

namespace BuildScripts;

[TaskName("Build macOS")]
[IsDependentOn(typeof(PrepTask))]
[IsDependeeOf(typeof(BuildLibraryTask))]
public sealed class BuildMacOSTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.IsRunningOnMacOs();

    public override void Run(BuildContext context)
    {
        var buildWorkingDir = "mojoshaderbuild/";
        context.CreateDirectory(buildWorkingDir);
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "../mojoshader/CMakeLists.txt -DCMAKE_OSX_DEPLOYMENT_TARGET=10.15  -DCMAKE_OSX_ARCHITECTURES=\"x86_64;arm64\"" });
        context.StartProcess("cmake", new ProcessSettings { WorkingDirectory = buildWorkingDir, Arguments = "--build . --config release" });
        context.CopyFile(System.IO.Path.Combine(buildWorkingDir, "libmojoshader.dylib"), $"{context.ArtifactsDir}/libmojoshader.dylib");
    }
}
