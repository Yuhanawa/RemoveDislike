using System.Collections.Generic;
using Cavitation.Core.Rule;
using static Cavitation.Core.Rule.Rule.ModeEnum;
using static Cavitation.Core.Utils;


namespace Cavitation.Core.Rules
{
    public class SystemCache : CacheModel
    {
        private readonly string Appdata = GetEnvironmentVar("Appdata");
        private readonly string SystemRoot = GetEnvironmentVar("SystemRoot");


        public SystemCache()
        {
            Rules = new List<Rule.Rule>
            {
                // new(@$""), // 
                new(@$"{Appdata}\..\Local\Microsoft\Windows\Explorer\", RecursionAllFiles, ".db"), //缩略图
                new(@$"{Appdata}\..\Local\Microsoft\Windows\Caches\", RecursionAllFiles, ".db"), //缓存
                new(@$"{Appdata}\..\Local\Microsoft\Windows\History\"), // 历史记录
                new(@$"{SystemRoot}\Temp\"), // 系统缓存
                new(@$"{SystemRoot}\ServiceProfiles\LocalService\AppData\Local\FontCache\", RecursionAllFiles,
                    "dat"), // 
                new(@$"{SystemRoot}\ServiceProfiles\LocalService\AppData\Local\Temp\") // 


                // $"{Appdata}\Local\Microsoft\Windows\WinX
                //
                // @$"{Appdata}\Local\Microsoft\Blend\BackupFiles", // Vs Blender backup
                // @$"{Appdata}\Local\Microsoft\VisualStudio\BackupFiles", // VS",
                //
                // @$"{Appdata}\Local\Microsoft\Edge\User Data\GrShaderCache", // Edge Cache",
                // @$"{Appdata}\Local\Microsoft\Edge\User Data\Profile 1\Cache",
                // @$"{Appdata}\Local\Microsoft\Edge\User Data\Profile 1\Code Cache",
                // @$"{Appdata}\Local\Microsoft\Edge\User Data\Profile 1\GPUCache",
                // @$"{Appdata}\Local\Microsoft\Edge\User Data\ShaderCache",
                //
                // @$"{Appdata}\Local\Microsoft\Internet Explorer\CacheStorage",
                // @$"{Appdata}\Local\Microsoft\Media Player\Transcoded Files Cache",
                // @$"{Appdata}\Local\Microsoft\Visual Studio\Font Cache",
                // @$"{Appdata}\Local\Microsoft\VisualStudio\Roslyn\Cache",
                // @$"{Appdata}\Local\Microsoft\VisualStudio Services\8.0\Cache",
                // @$"{Appdata}\Local\Microsoft\VisualStudio Services\8.0\Cache",
            };
        }
    }
}