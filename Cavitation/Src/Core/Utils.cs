using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Cavitation.Core
{
    public class Utils
    {
        private static readonly Dictionary<string, string> EnvironmentVar = new()
        {
            { "SystemRoot", Environment.GetEnvironmentVariable("SystemRoot") },
            { "Windir", Environment.GetEnvironmentVariable("windir") },
            { "HomeDrive", Environment.GetEnvironmentVariable("HomeDrive") },
            { "SystemDrive", Environment.GetEnvironmentVariable("SystemDrive") },
            { "ProgramFiles", Environment.GetEnvironmentVariable("ProgramFiles") },
            { "ProgramFiles(x86)", Environment.GetEnvironmentVariable("ProgramFiles(x86)") },
            { "CommonProgramFiles", Environment.GetEnvironmentVariable("CommonProgramFiles") },
            { "UserProFile", Environment.GetEnvironmentVariable("UserProFile") },
            { "HomePath", Environment.GetEnvironmentVariable("HomePath") },
            { "Appdata", Environment.GetEnvironmentVariable("Appdata") },
            { "Temp", Environment.GetEnvironmentVariable("Temp") }
        };

        //删除文件
        public bool WipeFile(string filename, int timesToWrite)
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

        public static string GetEnvironmentVar(string name)
        {
            if (!EnvironmentVar.ContainsKey(name))
                EnvironmentVar.Add(name, Environment.GetEnvironmentVariable(name));

            return EnvironmentVar[name];
        }
    }
}