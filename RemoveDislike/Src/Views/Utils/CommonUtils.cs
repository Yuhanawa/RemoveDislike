using System;
using System.Diagnostics;

namespace RemoveDislike.Views.Utils
{
    public static class CommonUtils
    {
        public static string Log(string m)
        {
            var output = $"[{DateTime.Now}] [View] {m}";
            Debug.WriteLine(output);
            return output;
        }
    }
}