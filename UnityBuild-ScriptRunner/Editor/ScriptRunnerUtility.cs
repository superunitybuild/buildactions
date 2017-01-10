using System.Diagnostics;
using System.IO;

namespace SuperSystems.UnityBuild
{

public static class ScriptRunnerUtility
{
    public static void RunScript(string scriptPath, string arguments)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = Path.GetFullPath(scriptPath);

        if (!string.IsNullOrEmpty(arguments))
            startInfo.Arguments = arguments;

        Process proc = Process.Start(startInfo);
        proc.WaitForExit();
    }
}

}
