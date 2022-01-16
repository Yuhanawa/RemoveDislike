namespace RemoveDislike.Core.Utils
{
    public static class SizeUtils
    {
        public static string ToString(long size)  {
            var unit = "B";
            if(size > 1024) {
                size /= 1024;
                unit = "KB";
            }
            if(size > 1024) {
                size /= 1024;
                unit = "MB";
            }
            if(size > 1024) {
                size /= 1024;
                unit = "GB";
            }
            if(size > 1024) {
                size /= 1024;
                unit = "TB";
            }
            if(size > 1024) {
                size /= 1024;
                unit = "PB";
            }
            if(size > 1024) {
                size /= 1024;
                unit = "EB";
            }
            if(size > 1024) {
                size /= 1024;
                unit = "ZB";
            }
            if(size > 1024) {
                size /= 1024;
                unit = "YB";
            }

            return $"{size} {unit}";
        }
    }
}