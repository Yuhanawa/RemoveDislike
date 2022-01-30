using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoveDislike.Core.Utils.AntPathMatching;

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
    /// <param name="fileSystem">File system to be used</param>
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
        directory.Replace("\\", "/").Split('/').ToList().ForEach(x =>
        {
            if (x == "..")
                directory = Path.GetDirectoryName(directory);
            else if (x != ".")
                directory = Path.Combine(directory, x);

        });
        directory = directory.Replace("\\", "/").TrimStart('/');
        
        string[] files = Directory.Exists(directory)
            ? DirectoryInfoUtils.TryGetAllFilePaths(directory)
            : Array.Empty<string>();

        foreach (string file in files.Select(x => x.Replace("\\", "/").TrimStart('/')))
        {
            string actualFile = file.TrimStart(directory.ToCharArray()).TrimStart('/');
            
            if (ant.IsMatch(ant.IgnoreCase?actualFile.ToLower():actualFile))
                yield return includeDirectoryPath
                    ? file
                    : actualFile;
            
        }
    }
}