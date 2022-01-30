using System.IO;
using System.Linq;

namespace RemoveDislike.Core.Utils
{
    public static class FsUntils
    {
        public static long TryDel(this FileSystemInfo file)
        {
            try
            {
                switch (file)
                {
                    case FileInfo fileInfo:
                        string fullName = fileInfo.FullName;
                        long fileSize = fileInfo.Length;
                        fileInfo.Delete();
                        Debug($"[TryDel] File: {fullName} Size: {fileSize}");
                        return fileSize;
                    case DirectoryInfo directoryInfo:
                    {
                        Debug($"[TryDel] Directory: {directoryInfo.FullName}");
                        long size
                            = directoryInfo.GetFiles().Sum(f => f.TryDel())
                              + directoryInfo.GetDirectories().Sum(d => d.TryDel());
                        try { directoryInfo.Delete(); } catch { }
                        return size;
                    }
                    default: return 0;
                }
            } catch(Exception e)
            { Warn($"[TryDel] {e}"); return 0; }
        }
    }
}