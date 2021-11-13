using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using static RemoveDislike.Core.Utils.CommonUtils;

namespace RemoveDislike.Core.Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// 擦除文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="timesToWrite"></param>
        /// <returns></returns>
        public static bool WipeFile(string filename, int timesToWrite)
        {
            try
            {
                if (!File.Exists(filename)) return false;
                //设置文件的属性为正常，这是为了防止文件是只读 
                File.SetAttributes(filename, FileAttributes.Normal);
                //计算扇区数目 
                double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);
                // 创建一个同样大小的虚拟缓存 
                var dummyBuffer = new byte[512];
                // 创建一个加密随机数目生成器 
                var rng = new RNGCryptoServiceProvider();
                // 打开这个文件的FileStream 
                var inputStream = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                for (var currentPass = 0; currentPass < timesToWrite; currentPass++)
                {
                    // 文件流位置 
                    inputStream.Position = 0;
                    //循环所有的扇区 
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
                // 清空原始日期需要 
                var dt = new DateTime(2037, 1, 1, 0, 0, 0);
                File.SetCreationTime(filename, dt);
                File.SetLastAccessTime(filename, dt);
                File.SetLastWriteTime(filename, dt);
                // 删除文件 
                File.Delete(filename);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DelDir(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles())
                TryDelFile(file);

            foreach (DirectoryInfo info in dir.GetDirectories())
                DelDir(info);

            dir.Delete();
        }

        public static DelInfo TryDelFile(string path, bool output) => TryDelFile(new FileInfo(path), output);

        public static DelInfo TryDelFile(FileInfo file, bool output)
        {
            DelInfo info = TryDelFile(file);
            if (output)
                Log(info.ToString());

            return info;
        }

        public static DelInfo TryDelFile(FileInfo file)
        {
            try
            {
                var info = new DelInfo(true, file.FullName, file.Length);
                file.Delete();
                return info;
            }
            catch (Exception e)
            {
                try
                {
                    return new DelInfo(false, file.FullName, file.Length, e);
                }
                catch
                {
                    return new DelInfo(false, file.FullName, -1, e);
                }

            }
        }

        public static DelInfo TryExDelDir(DirectoryInfo dir, bool output)
        {
            var size = 0L;
            try
            {
                size += dir.GetFiles().Sum(file => TryDelFile(file.ToString(), true).Size);
            }
            catch (Exception e)
            {
                return new DelInfo(false, dir.FullName, size, e);
            }

            try
            {
                foreach (DirectoryInfo info in dir.GetDirectories())
                    TryExDelDir(info, output);
            }
            catch (Exception e)
            {
                return new DelInfo(false, dir.FullName, size, e);
            }

            try
            {
                dir.Delete();
            }
            catch (Exception e)
            {
                return new DelInfo(true, dir.FullName, size, e);
            }

            return new DelInfo(true, dir.FullName, size);
        }

        public static DelInfo TryDelDir(DirectoryInfo dir)
        {
            try
            {
                dir.Delete();
                return new DelInfo(true, dir.FullName, 0);
            }
            catch (Exception e)
            {
                return new DelInfo(true, dir.FullName, 0, e);
            }
        }

        private static void Del(FileSystemInfo fileSystemInfo, bool output)
        {
            switch (fileSystemInfo)
            {
                case DirectoryInfo dir:
                {
                    TryExDelDir(dir, output);
                    break;
                }
                case FileInfo file:
                {
                    if (output) Log(TryDelFile(file).ToString());
                    else TryDelFile(file);
                    break;
                }
            }
        }

        public class DelInfo
        {
            public Exception Exception;
            public bool IsSuccess;
            public long Size;
            public string Source;

            public DelInfo()
            {
            }

            public DelInfo(bool isSuccess, string source)
            {
                IsSuccess = isSuccess;
                Source = source;
            }

            public DelInfo(bool isSuccess, string source, long size)
            {
                IsSuccess = isSuccess;
                Source = source;
                Size = size;
            }

            public DelInfo(bool isSuccess, string source, Exception exception)
            {
                IsSuccess = isSuccess;
                Source = source;
                Exception = exception;
            }

            public DelInfo(bool isSuccess, string source, long size, Exception exception)
            {
                IsSuccess = isSuccess;
                Source = source;
                Size = size;
                Exception = exception;
            }

            public string ToString(bool isSentence)
            {
                if (isSentence)
                    return IsSuccess
                        ? $"File deleted successfully: {Source} ; Size: {Size}. "
                        : $"File deleted fail: {Source} ; Size: {Size} ; Exception: {Exception}. ";

                return ToString();
            }

            public override string ToString() => IsSuccess
                ? $"[File] [{(int)(Size / 1024)} MB] {Source} ; Size: {Size} KB. "
                : $"[File] [Warn] {Exception.Message} ; Source: {Source} ; Size: {Size} KB. ";
        }

        /// <summary>
        /// 检查当前用户是否拥有此文件夹的操作权限
        /// Check whether the current user has operation permissions for this file or folder
        /// </summary>
        /// <param name="path">File or Folder Path</param>
        /// <param name="isFolder"></param>
        /// <see>
        ///     <cref>https://blog.csdn.net/weixin_34391445/article/details/86017021</cref>
        /// </see>
        /// <returns></returns>
        public static bool HasOperationPermission(string path, bool isFolder) =>
            isFolder
                ? File.GetAccessControl(path)
                    .GetAccessRules(true, true, typeof(NTAccount))
                    .OfType<FileSystemAccessRule>()
                    .Where(i =>
                        i.IdentityReference.Value == Path.Combine(Environment.UserDomainName, Environment.UserName))
                    .ToList()
                    .Any(i => i.AccessControlType == AccessControlType.Deny)
                : File.GetAccessControl(path)
                    .GetAccessRules(true, true, typeof(NTAccount))
                    .OfType<FileSystemAccessRule>()
                    .Where(i =>
                        i.IdentityReference.Value == Path.Combine(Environment.UserDomainName, Environment.UserName))
                    .ToList()
                    .Any(i => i.AccessControlType == AccessControlType.Deny);
    }
}