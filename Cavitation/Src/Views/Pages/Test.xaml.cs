using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cavitation.Core.Rules;

namespace Cavitation.Views.Pages
{
    public partial class Test : Page
    {
        public List<string> NameList = new()
        {
            "log", // 日志
            "logs", // 日志
            "tmp", // 缓存
            "temp", // 缓存
            "cache"

            //%systemdrive%\recycled\*.*
            // %windir%\prefetch\*.*  // 系统预读取文件 可能堆积成山
            // %windir%\temp
            //del /f /q %userprofile%\cookies\*.*
            //del /f /q %userprofile%\recent\*.*
        };

        public List<string> SuffixList = new()
        {
            ".log", // 日志
            ".tmp", // 缓存
            ".temp", // 缓存
            "._mp", // 缓存
            ".gid", // 引索
            ".chk" // 可能可恢复的损坏的碎片文件
            // ".old", // 老文件
            // ".bak", // 备份文件 可能被遗弃
            // ".back", // 备份文件 可能被遗弃
            // ".cache",

            //%systemdrive%\recycled\*.*
            // %windir%\prefetch\*.*  // 系统预读取文件 可能堆积成山
            // %windir%\temp
            //del /f /q %userprofile%\cookies\*.*
            //del /f /q %userprofile%\recent\*.*
        };

        public Test()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确认清理", "确认清理", MessageBoxButton.YesNo) == MessageBoxResult.Yes) Clear();
            new SystemCache().Clear();
            MessageBox.Show("done.", "done.");
        }

        private void Clear()
        {
            try
            {
                RecursiveFolder(@"c:\");
                Debug.WriteLine("done.");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // throw;
            }
        }

        private void RecursiveFolder(string path)
        {
            FileSystemInfo[] fileInfos = new DirectoryInfo(path).GetFileSystemInfos();
            if (fileInfos.Length == 0)
            {
                Directory.Delete(path);
                Debug.WriteLine($"[Folder] {path}  Empty");
                return;
            }

            foreach (FileSystemInfo fileInfo in fileInfos)
            {
                bool isDir = Directory.Exists(fileInfo.FullName);
                if (NameList.Any(name => fileInfo.Name.ToLower() == name || fileInfo.Name.ToLower() == '.' + name))
                {
                    string type = isDir ? "Folder" : "file";
                    Debug.WriteLine($"[{type}] {fileInfo.FullName}  name: {fileInfo.Name}");
                }

                if (!fileInfo.Exists) break;
                if (!Directory.Exists(fileInfo.FullName))
                {
                    if (SuffixList.All(suffix => fileInfo.Extension != suffix)) continue;
                    fileInfo.Delete();
                    Debug.WriteLine("[File] " + fileInfo.FullName + "  suffix: " + fileInfo.Extension);
                }
                else
                {
                    // Debug.WriteLine("[Folder] " + fileInfo.FullName);
                    try
                    {
                        RecursiveFolder(fileInfo.FullName);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"[warn] {e.Message}");
                    }
                }
            }
        }
    }
}