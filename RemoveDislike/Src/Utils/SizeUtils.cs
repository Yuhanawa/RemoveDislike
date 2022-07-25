namespace RemoveDislike.Utils;

public static class SizeUtils
{
    public static string ToString(long size)
    {
        var unit = "B";
        double result = size;
        if (result > 1024)
        {
            result /= 1024F;
            unit = "KB";
        }

        if (result > 1024)
        {
            result /= 1024;
            unit = "MB";
        }

        if (result > 1024)
        {
            result /= 1024;
            unit = "GB";
        }

        if (result > 1024)
        {
            result /= 1024;
            unit = "TB";
        }

        if (result > 1024)
        {
            result /= 1024;
            unit = "PB";
        }

        if (result > 1024)
        {
            result /= 1024;
            unit = "EB";
        }

        if (result > 1024)
        {
            result /= 1024;
            unit = "ZB";
        }

        if (result > 1024)
        {
            result /= 1024;
            unit = "YB";
        }

        return $"{result:F2} {unit}";
    }
}