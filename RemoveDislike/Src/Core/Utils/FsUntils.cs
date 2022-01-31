using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace RemoveDislike.Core.Utils;

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
                    try
                    {
                        directoryInfo.Delete();
                    }
                    catch
                    {
                    }

                    return size;
                }
                default: return 0;
            }
        }
        catch (Exception e)
        {
            Warn($"[TryDel] {e}");
            return 0;
        }
    }

    public static void WipeFile(string filename, int timesToWrite)
    {
        try
        {
            if (!File.Exists(filename)) return;
            //设置文件的属性为正常，这是为了防止文件是仅仅读
            File.SetAttributes(filename, FileAttributes.Normal);
            //计算扇区数目
            double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);
            // 创建一个相同大小的虚拟缓存
            var dummyBuffer = new byte[512];
            // 创建一个加密随机数目生成器
            var rng = RandomNumberGenerator.Create();
            // RNGCryptoServiceProvider rng = new();
            // 打开这个文件的FileStream
            FileStream inputStream = new(filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            for (var currentPass = 0; currentPass < timesToWrite; currentPass++)
            {
                // 文件流位置
                inputStream.Position = 0;
                //循环全部的扇区
                for (var sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                {
                    //把垃圾数据填充到流中
                    rng.GetBytes(dummyBuffer);
                    // 写入文件流中
                    inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                }
            }

            // 清空文件
            inputStream.SetLength(0);
            // 关闭文件流
            inputStream.Close();
            // 清空原始日期须要
            DateTime dt = new(2037, 1, 1, 0, 0, 0);
            File.SetCreationTime(filename, dt);
            File.SetLastAccessTime(filename, dt);
            File.SetLastWriteTime(filename, dt);
            // 删除文件
            File.Delete(filename);
        }
        catch (Exception e)
        {
            Warn($"[WipeFile] {e}");
        }
    }
}