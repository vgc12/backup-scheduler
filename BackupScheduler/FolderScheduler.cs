using System.IO.Compression;
using System.Timers;

namespace BackupScheduler;

public class FolderScheduler(Dictionary<string, string> save, double intervalInMilliseconds)
    : IScheduler
{
    public Dictionary<string, string> SaveAndOutputDirectories { get; set; } = save;

    public bool UseAsync { get; set; }
    public string FormattedDateTimeNow => $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";

    public double IntervalInMilliseconds { get; set; } = intervalInMilliseconds;

    public void Start()
    {
        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Elapsed += OnTimedEvent;
        aTimer.Interval = IntervalInMilliseconds;
        aTimer.Enabled = true;
        
         
        Console.WriteLine("Press \'q\' to stop the scheduler.");
        while(Console.Read() != 'q');
    }

    private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        try
        {
            await BackUpDirectoryAsync();
            
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private async Task BackUpDirectoryAsync()
    {
        var tasks = new List<Task>();

        foreach (var item in SaveAndOutputDirectories)
        {
            string startPath = item.Key;
            string zipPath = Path.Combine(item.Value, Path.GetFileName(Path.GetDirectoryName(item.Key)) + "_" + FormattedDateTimeNow + ".zip");

            tasks.Add(Task.Run(() =>
            {
                try
                {
                    ZipFile.CreateFromDirectory(startPath, zipPath);
                    Console.WriteLine($"Directory {startPath} backed up to {zipPath}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }));
        }

        await Task.WhenAll(tasks); 
    }

   
}