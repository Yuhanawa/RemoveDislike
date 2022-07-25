using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoveDislike.Utils.AntPathMatching;

/// <summary>
///     Represents a class used to match files recusively with ant-style pattern.
/// </summary>
public class AntDirectory
{
    private readonly Ant ant;

    /// <summary>
    ///     Initializes a new <see cref="AntDirectory" />.
    /// </summary>
    /// <param name="ant">Ant pattern used for directory-searching.</param>
    /// <exception cref="ArgumentNullException">Throw when <paramref name="ant" /> is null.</exception>
    public AntDirectory(Ant ant) =>
        this.ant = ant ?? throw new ArgumentNullException(nameof(ant));

    /// <summary>
    ///     Searches all the files in the given directory using the ant-style pattern.
    /// </summary>
    /// <param name="directory">Path to directory to search in.</param>
    /// <param name="includeDirectoryPath">Indicates if the returned paths must include the directory.</param>
    /// <returns>Collection of matching files.</returns>
    /// <inheritDoc />
    public IEnumerable<string> SearchRecursively(string directory, bool includeDirectoryPath = false)
    {
        directory = PathFormatting(directory);

        string[] files = Directory.Exists(directory)
            ? DirectoryInfoUtils.TryGetAllFilePaths(directory)
            : Array.Empty<string>();

        foreach (string file in files.Select(GetUnixPath))
        {
            string actualFile = file.TrimStart(directory!.ToCharArray()).TrimStart('/');

            if (ant.IsMatch(ant.IgnoreCase ? actualFile.ToLower() : actualFile))
            {
                yield return includeDirectoryPath
                    ? file
                    : actualFile;
            }
        }
    }

    /// <summary>
    ///     Searches all the files in the given directory using the ant-style pattern.
    /// </summary>
    /// <param name="directory">Path to directory to search in.</param>
    /// <param name="action"></param>
    /// <param name="includeDirectoryPath">Indicates if the returned paths must include the directory.</param>
    public void SearchRecursively(string directory, Action<string> action, bool includeDirectoryPath = false)
    {
        directory = PathFormatting(directory);

        if (!Directory.Exists(directory)) return;


        DirectoryInfoUtils.TryGetAllFilePaths(directory, file =>
        {
            file = GetUnixPath(file);
            string actualFile = file.TrimStart(directory!.ToCharArray()).TrimStart('/');

            if (ant.IsMatch(ant.IgnoreCase ? actualFile.ToLower() : actualFile))
                action(includeDirectoryPath ? file : actualFile);
        });
    }

    private static string GetUnixPath(string path) => path.Replace(@"\", "/").TrimStart('/');

    private static string PathFormatting(string path)
    {
        GetUnixPath(path).Split('/').ToList().ForEach(x =>
        {
            if (x == "..")
                path = Path.GetDirectoryName(path);
            else if (x != ".")
                path = Path.Combine(path, x);
        });
        return GetUnixPath(path);
    }
}