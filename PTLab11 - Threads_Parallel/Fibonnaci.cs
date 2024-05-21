using System.ComponentModel;
using System.Numerics;
using Gtk;

namespace PTLab11___Threads_Parallel;

public class Fibonnaci : Window
{
    private readonly BackgroundWorker _backgroundWorker;
    private readonly Button _cancelButton;
    private readonly Entry _fibonacciInput;
    private readonly ProgressBar _progressBar;
    private readonly Button _startButton;

    public Fibonnaci() : base("Progress Bar Example")
    {
        SetDefaultSize(400, 200);
        SetPosition(WindowPosition.Center);

        var vbox = new Box(Orientation.Vertical, 5);
        _progressBar = new ProgressBar
        {
            ShowText = true
        };
        vbox.PackStart(_progressBar, true, true, 0);
        _fibonacciInput = new Entry();
        vbox.PackStart(_fibonacciInput, false, false, 0);

        _startButton = new Button("Start");
        _startButton.Clicked += OnStartButtonClicked;
        vbox.PackStart(_startButton, false, false, 0);

        _cancelButton = new Button("Cancel");
        _cancelButton.Clicked += OnCancelButtonClicked;
        vbox.PackStart(_cancelButton, false, false, 0);

        Add(vbox);
        ShowAll();

        _backgroundWorker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        _backgroundWorker.DoWork += BackgroundWorker_DoWork;
        _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
        _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        DeleteEvent += (o, args) => Application.Quit();
    }

    private void OnStartButtonClicked(object? sender, EventArgs e)
    {
        _startButton.Sensitive = false;
        _cancelButton.Sensitive = true;

        if (BigInteger.TryParse(_fibonacciInput.Text, out var fibonacciNumber))
        {
            _progressBar.Text = "";
            _backgroundWorker.RunWorkerAsync(fibonacciNumber);
        }
        else
        {
            _progressBar.Text = "Invalid input!";
            _startButton.Sensitive = true;
            _cancelButton.Sensitive = false;
        }
    }

    private void OnCancelButtonClicked(object? sender, EventArgs e)
    {
        if (_backgroundWorker.WorkerSupportsCancellation) _backgroundWorker.CancelAsync();
    }

    private static void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        var worker = sender as BackgroundWorker;
        var fibonacciNumber = e.Argument as BigInteger?;
        if (fibonacciNumber == 0 || fibonacciNumber == 1)
        {
            Console.WriteLine("Fibonnaci number: 1");
            e.Result = 1;
            return;
        }

        BigInteger value = 1;
        BigInteger lastValue = 1;
        for (var i = 2; i < fibonacciNumber; i++)
        {
            if (worker is { CancellationPending: true })
            {
                e.Cancel = true;
                return;
            }

            var temp = value;
            value = value + lastValue;
            lastValue = temp;


            Thread.Sleep(5);
            var progress = (double)(i + 1) * 100 / (double)fibonacciNumber;
            worker?.ReportProgress((int)progress);
            
        }

        Console.WriteLine($"Fibonacci number: {value}");
        e.Result = value;
    }

    private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        _progressBar.Fraction = e.ProgressPercentage / 100.0;
        _progressBar.Text = $"{e.ProgressPercentage}%";
    }

    private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        _startButton.Sensitive = true;
        _cancelButton.Sensitive = false;

        if (e.Cancelled)
            _progressBar.Text = "Operation was canceled.";
        else if (e.Error != null)
            _progressBar.Text = $"Error: {e.Error.Message}";
        else
            _progressBar.Text = "Operation completed successfully.";
    }
}