using System.Diagnostics;

namespace RemoveDislike.Core.Utils
{
    public static class LogUtils
    {
        public static string Log(string m)
        {
            var output = $"[{DateTime.Now}] {m}";
            Debug.WriteLine(output);
            return output;
        }

        public static string Info(string m) => Log($"[Info] {m}");

        public static string Warn(string m) => Log($"[Warn] {m}");

        public static string Err(string m, Exception e) => Log($"[Err] {m} Exception: {e}");

        public static string Fatal(string m, Exception e) => Log($"[Fatal] {m} Exception: {e}");
    }
}