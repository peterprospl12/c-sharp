using System.IO;
using System.Windows.Controls;

namespace PTLab07;

public class Program
{

    public static void PrintDirectories(string path, TreeViewItem treeNode)
    {
        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);

        foreach (var directory in directories)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name, Tag = directory };
            treeNode.Items.Add(directoryNode);
            PrintDirectories(directory, directoryNode);
        }

        foreach (var file in files)
        {
            var fileNode = new TreeViewItem { Header = $"{Path.GetFileName(file)}", Tag = $"{file}" };
            treeNode.Items.Add(fileNode);
        }
    }

    [Serializable]
    private class StringComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null || y == null) throw new ArgumentNullException(x == null ? nameof(x) : nameof(y));

            return x.Length != y.Length ? x.Length.CompareTo(y.Length) : string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}