
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoveDislike.Core.Utils;

public static class DirectoryInfoUtils
{
    /// <summary>
    /// Try to recursive traversal all FileInfos in the directory ( ignore errors )
    /// </summary>
    /// <param name="directoryInfo"></param>
    /// <returns>all FileInfos</returns>
    public static FileInfo[] TryGetAllFiles(this DirectoryInfo directoryInfo)
    {
        List<FileInfo> files = new();
        try {
            files.AddRange(directoryInfo.GetFiles());
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                try { files.AddRange(directory.TryGetAllFiles()); }
                catch { /* ignore errors */}
        } catch { /* ignore errors */ }
        
        return files.ToArray();
    }
    
    /// <summary>
    /// Try to recursive traversal all path of files in the directory ( ignore errors )
    /// </summary>
    /// <param name="directoryInfo"></param>
    /// <returns>all path of files</returns>
    public static string[] TryGetAllFilePaths(this DirectoryInfo directoryInfo)
    {
        List<string> files = new();
        try {
            files.AddRange(directoryInfo.GetFiles().Select(file => file.FullName));
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                try { files.AddRange(directory.TryGetAllFilePaths()); }
                catch { /* ignore errors */}
        } catch { /* ignore errors */ }
        
        return files.ToArray();
    }
    
    /// <summary>
    ///  Try to recursive traversal all path of files in the directory by directory path( ignore errors )
    /// </summary>
    /// <param name="path">directory path</param>
    /// <returns>all path of files</returns>
    public static string[] TryGetAllFilePaths(string path) => new DirectoryInfo(path).TryGetAllFilePaths();
    
    /// <summary>
    ///  Try to recursive traversal all path of files in the directory by directory path( ignore errors )
    /// </summary>
    /// <param name="path">directory path</param>
    /// <param name="action"></param>
    public static void TryGetAllFilePaths(string path, Action<string> action)
    {
        try
        {
            try { Directory.GetFiles(path).ToList().ForEach(action); }
            catch (Exception e) { /* ignore errors */ }

            foreach (string directory in Directory.GetDirectories(path))
                try { TryGetAllFilePaths(directory, action); }
                catch { /* ignore errors */ }
        }
        catch { /* ignore errors */ }
    }
}