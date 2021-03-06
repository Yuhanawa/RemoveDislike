using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RemoveDislike.Utils.AntPathMatching;

/// <summary>
///     Represents a class which matches paths using ant-style path matching.
/// </summary>
/// <seealso cref="AntPathMatching.Ant" />
[DebuggerDisplay("Pattern = {regex}")]
public class Ant
{
    public readonly bool IgnoreCase;
    private readonly string originalPattern;
    private readonly Regex regex;

    /// <summary>
    ///     Initializes a new <see cref="Ant" />.
    /// </summary>
    /// <param name="pattern">Ant-style pattern.</param>
    /// <param name="ignoreCase">ignore Case(default:false)</param>
    public Ant(string pattern, bool ignoreCase = false)
    {
        IgnoreCase = ignoreCase;
        pattern = ignoreCase ? pattern.ToLower() : pattern;
        originalPattern = pattern ?? string.Empty;
        regex = new Regex(
            EscapeAndReplace(originalPattern),
            RegexOptions.Singleline
        );
    }

    /// <summary>
    ///     Validates whether the input matches the given pattern.
    /// </summary>
    /// <param name="input">Path for which to check if it matches the ant-pattern.</param>
    /// <returns>Whether the input matches the pattern.</returns>
    public bool IsMatch(string input)
    {
        input ??= string.Empty;
        return regex.IsMatch(GetUnixPath(input));
    }

    private static string EscapeAndReplace(string pattern)
    {
        string unix = GetUnixPath(pattern);

        if (unix.EndsWith("/")) unix += "**";

        pattern = unix
            .Replace(@"*", "([^/]*)")
            .Replace(@"([^/]*)([^/]*)", "(.*)")
            .Replace(@"?", "(.)")
            .Replace(@"}", ")")
            .Replace(@"{", "(")
            .Replace(@",", "|");

        return $"^{pattern}$";
    }

    private static string GetUnixPath(string txt) => txt.Replace(@"\", "/").TrimStart('/');

    /// <summary>
    ///     Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => originalPattern;
}