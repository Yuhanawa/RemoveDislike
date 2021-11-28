using System.Collections.Generic;
using System.IO;
using fastJSON;
using static System.Threading.Thread;
using static RemoveDislike.Core.Utils.LogUtils;

namespace RemoveDislike.Core.Utils
{
    public static class I18NUtils
    {
        private static Dictionary<string, string> I18N { get; set; } = new();

        /// <summary>
        ///     Get the translated string by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns> if the key is found return translated string, else return key </returns>
        public static string Get(string key) =>
            I18N.ContainsKey(key) ? I18N[key] : key;

        /// <summary>
        ///     Load the i18n file
        /// </summary>
        public static void Load()
        {
            Info("Loading the i18n file...");
            try
            {
                string language = CurrentThread.CurrentCulture.Name;
                LoadLanguage(language);
            }
            catch
            {
                Warn("Loaded i18n file fail, will use English Language");
                try
                {
                    LoadLanguage("en");
                }
                catch
                {
                    // ignored
                }
            }

            Info($"Successfully loaded i18n{Get("@lang")} files, a total of {I18N.Count} translations");
        }

        /// <summary>
        ///     LoadLanguage
        /// </summary>
        /// <param name="language">
        ///     like "zh-CN"
        ///     You can get the language code here:
        ///     { System.Threading.Thread.CurrentThread.CurrentCulture.Name }
        /// </param>
        /// <exception cref="FileNotFoundException"> Language file not found </exception>
        private static void LoadLanguage(string language)
        {
            string path = Path.Combine(ConfigHelper.ConfigPath, "i18n", $"{language}.json");
            if (!File.Exists(path))
                throw new FileNotFoundException("Language file not found");

            I18N = JSON.ToObject<Dictionary<string, string>>(File.ReadAllText(path));
        }
    }
}