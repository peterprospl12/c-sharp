using System.IO.Enumeration;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Mono.Unix;

namespace PTLab07;

internal class Program
{
    private static string ConvertFilePermissions(FileAccessPermissions permissions)
    {
        var result = new StringBuilder("----------");

        if ((permissions & FileAccessPermissions.UserRead) != 0)
            result[1] = 'r';
        if ((permissions & FileAccessPermissions.UserWrite) != 0)
            result[2] = 'w';
        if ((permissions & FileAccessPermissions.UserExecute) != 0)
            result[3] = 'x';

        if ((permissions & FileAccessPermissions.GroupRead) != 0)
            result[4] = 'r';
        if ((permissions & FileAccessPermissions.GroupWrite) != 0)
            result[5] = 'w';
        if ((permissions & FileAccessPermissions.GroupExecute) != 0)
            result[6] = 'x';

        if ((permissions & FileAccessPermissions.OtherRead) != 0)
            result[7] = 'r';
        if ((permissions & FileAccessPermissions.OtherWrite) != 0)
            result[8] = 'w';
        if ((permissions & FileAccessPermissions.OtherExecute) != 0)
            result[9] = 'x';

        return result.ToString();
    }

    private static void PrintDirectories(string path, SortedList<string, int> filesList, int depth = 0)
    {
        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        (string FilePath, DateTime Date) oldestFile = ("", DateTime.MaxValue);

        for (var i = 0; i < depth; i++) Console.Write("\t");
        Console.WriteLine(
            $"{Path.GetFileName(path)} ({directories.Length + files.Length}) {(directories.Length + files.Length != 0 ? "----" : "")}");

        var directoryInfo = new DirectoryInfo(path);
        filesList[directoryInfo.Name] = directories.Length + files.Length;

        foreach (var directory in directories) PrintDirectories(directory, filesList, depth + 1);
        foreach (var file in files)
        {
            for (var i = 0; i < depth + 1; i++)
                Console.Write("\t");
            var fileInfo = new FileInfo(file);
            var unixFileInfo = new UnixFileInfo(fileInfo.FullName);

            var dataTime = fileInfo.CreationTime;
            if (dataTime < oldestFile.Date)
            {
                oldestFile.FilePath = fileInfo.FullName;
                oldestFile.Date = dataTime;
            }

            filesList[fileInfo.Name] = (int)fileInfo.Length;
            Console.WriteLine(Path.GetFileName(file) + " " + fileInfo.Length + " bytes " +
                              ConvertFilePermissions(unixFileInfo.FileAccessPermissions));
        }

        if (depth != 0) return;
        var formatter = new BinaryFormatter();
        using var stream = new FileStream("filesList.bin", FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, filesList);
        Console.WriteLine("\nOldest file: " + oldestFile + "\n");
    }

    private static void Main(string[] args)
    {
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
        var filesList = new SortedList<string, int>();
        PrintDirectories("/home/piotr/Downloads/maslo", filesList);
        var formatter = new BinaryFormatter();
        using var stream = new FileStream("filesList.bin", FileMode.Open, FileAccess.Read);
        var newFilesList = (SortedList<string, int>)formatter.Deserialize(stream);
        foreach (var pair in newFilesList) Console.WriteLine(pair.Key + " -> " + pair.Value);

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