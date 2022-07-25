using System.Diagnostics;

namespace RemoveDislike.Utils;

public static class ProcessUtils
{
    public static void TryStart(string fileName, string arguments)
    {
        try
        {
            Process.Start(fileName, arguments);
        }
        catch (Exception ex)
        {
            Err($"Try to start process {fileName} with arguments {arguments} failed: {ex.Message}", ex);
        }
    }

    public static void TryStart(string fileName)
    {
        try
        {
            Process.Start(fileName);
        }
        catch (Exception ex)
        {
            Err($"Try to start process {fileName} without arguments failed: {ex.Message}", ex);
        }
    }
}