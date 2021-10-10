using System;
using System.Diagnostics;

namespace Cavitation.Core.Utils
{
    public static class CommonUtils
    {
        public static string Log(string m)
        {
            var output = $"[{DateTime.Now}] {m}";
            Debug.WriteLine(output);
            return output;
        }
    }
}