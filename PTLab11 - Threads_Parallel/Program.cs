using System.IO.Compression;
using Gtk;

namespace PTLab11___Threads_Parallel;

internal class Program
{
    private static double CalcFactorial(int n, int k)   
    {
        var result = 1.0;
        for (var i = k; i <= n; i++) result *= i;

        return result;
    }

    private static async Task<double> CalcFactorialAsync(int n, int k)
    {
        return await Task.Run(() =>
        {
            var result = 1.0;
            for (var i = k; i <= n; i++) result *= i;

            return result;
        });
    }


    private static double NewtonBinomialTasks(int n, int k)
    {
        var numeratorTask = Task.Run(() => CalcFactorial(n, n - k + 1));
        var denominatorTask = Task.Run(() => CalcFactorial(k, 2));
        Task.WhenAll(numeratorTask, denominatorTask);
        return numeratorTask.Result / denominatorTask.Result;
    }

    private static double NewtonBinomialDelegates(int n, int k)
    {
        // var numeratorDelegate = CalcFactorial;
        // var denominatorDelegate = CalcFactorial;
        //
        // IAsyncResult numeratorResult = numeratorDelegate.BeginInvoke(n, n - k + 1, null, null);
        // IAsyncResult denominatorResult = denominatorDelegate.BeginInvoke(k, 2, null, null);
        //
        // numeratorResult.AsyncWaitHandle.WaitOne();
        // denominatorResult.AsyncWaitHandle.WaitOne();
        //
        // var numerator = numeratorDelegate.EndInvoke(numeratorResult);
        // var denominator = denominatorDelegate.EndInvoke(denominatorResult);
        //
        // return numerator / denominator;
        return 0;
    }

    private static async Task<double> NewtonBinomialAsync(int n, int k)
    {
        var numeratorTask = CalcFactorialAsync(n, n - k + 1);
        var denominatorTask = CalcFactorialAsync(k, 2);

        var results = await Task.WhenAll(numeratorTask, denominatorTask);

        var numerator = results[0];
        var denominator = results[1];

        return numerator / denominator;
    }

    private static async Task Main(string[] args)
    {
        var n = 5;
        var k = 3;


        var x = NewtonBinomialTasks(n, k);
        var y = NewtonBinomialDelegates(n, k);
        var z = NewtonBinomialAsync(n, k);
        Console.WriteLine("Tasks: " + x);
        Console.WriteLine("Delegates: " + y);
        Console.WriteLine("AsyncAwait: " + await z);
        Application.Init();
        var fibonnaci = new Fibonnaci();
        fibonnaci.Show();
        Application.Run();

        var selectedPath = SelectFolder();
        if (!string.IsNullOrEmpty(selectedPath))
        {
            Console.WriteLine($"Selected folder: {selectedPath}");
            var files = Directory.GetFiles(selectedPath);
            Parallel.ForEach(files, CompressFile);

            var compressedFiles = Directory.GetFiles(selectedPath, "*.gz");
            Parallel.ForEach(compressedFiles, DecompressFile);

            Console.WriteLine("Compression and decompression finished.");
        }
    }

    private static string SelectFolder()
    {
        string? folderPath = null;

        var dialog = new FileChooserDialog(
            "Choose directory",
            null,
            FileChooserAction.SelectFolder,
            "Cancel", ResponseType.Cancel,
            "Choose", ResponseType.Accept);

        if (dialog.Run() == (int)ResponseType.Accept) folderPath = dialog.Filename;

        dialog.Destroy();
        return folderPath;
    }

    private static void CompressFile(string filePath)
    {
        using var originalFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var compressedFileStream = new FileStream(filePath + ".gz", FileMode.Create, FileAccess.Write);
        using var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        originalFileStream.CopyTo(compressionStream);
    }

    private static void DecompressFile(string filePath)
    {
        using var compressedFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var decompressedFileStream =
            new FileStream(Path.ChangeExtension(filePath, null), FileMode.Create, FileAccess.Write);
        using var decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress);
        decompressionStream.CopyTo(decompressedFileStream);
    }
}