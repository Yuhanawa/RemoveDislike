namespace RemoveDislike.Core.Utils;

public static class LogUtils
{
    public static string Log(string m) => Log("", m);

    private static string Log(string header, string m, string suffix = "")
    {
        header = $"[{DateTime.Now}] {header} ";
        string output = $"{header}{m} {suffix}".Replace("\n", "\n" + header);
        System.Diagnostics.Debug.WriteLine(output);
        return output;
    }

    public static string Debug(string m) => Log("[Debug]", m);
    public static string Info(string m) => Log("[Info]", m);

    public static string Warn(string m) => Log("[Warn]", m);

    public static string Err(string m, Exception e) => Log("[Err]", m, $"Exception: {e}");

    public static string Fatal(string m, Exception e) => Log("[Fatal]", m, $"Exception: {e}");
}