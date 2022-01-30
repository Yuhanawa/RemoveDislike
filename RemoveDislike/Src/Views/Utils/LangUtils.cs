using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Resources;

namespace RemoveDislike.Views.Utils;

/// <summary>
///     LangUtils
/// </summary>
public class LangUtils : MarkupExtension
{
    public LangUtils(string key) => Key = key;
    public static Dictionary<string, string> Lang { get; } = new();

    public string Key { get; set; }


    public static string Get(string key) => Lang.ContainsKey(key) ? Lang[key] : key;

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        Lang.ContainsKey(Key) ? Lang[Key] : Key;

    public static void FromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        FromLines(lines);
    }

    public static void FromResource(Uri uri)
    {
        StreamResourceInfo sri = Application.GetResourceStream(uri);
        if (sri == null) return;
        StreamReader sr = new(sri.Stream);
        string str = sr.ReadToEnd();
        FromString(str);
    }

    public static void FromString(string str) => FromLines(str.Split('\n'));

    public static void FromLines(IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            string[] split = line.Split('=');
            if (split.Length != 2) continue;
            Lang.Add(split[0].Trim(), split[1].Trim());
        }
    }

    public static void Load(string lang)
    {
        Lang.Clear();
        if (lang == "en-US") return;
        Uri uri = new(@"Resources/Lang/" + lang + ".lang", UriKind.RelativeOrAbsolute);
        FromResource(uri);
    }
}

/// <summary>
///     In order to write xaml more convenient
/// </summary>
public class Lang : LangUtils
{
    public Lang(string key) : base(key)
    {
    }
}