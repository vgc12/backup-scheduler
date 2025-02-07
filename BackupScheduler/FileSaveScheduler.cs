using System.Text;
using System.Timers;

namespace BackupScheduler;

public class FileSaveScheduler(Dictionary<string, string> filesAndOutputDirectories, double intervalInMilliseconds) : IScheduler
{
    public Dictionary<string, string> FilesAndOutputDirectories { get; set; } = filesAndOutputDirectories;
    
    public double IntervalInMilliseconds { get; set; } = intervalInMilliseconds;
    
    public string FormattedDateTimeNow => $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
    
    public void Start()
    {
        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Elapsed += OnTimedEvent;
        aTimer.Interval = IntervalInMilliseconds;
        aTimer.Enabled = true;
        
         
        Console.WriteLine("Press \'q\' to stop the scheduler.");
        while (Console.Read() != 'q') ;
    }

    private async void OnTimedEvent(object? sender, ElapsedEventArgs e)
    {
        try
        {
            await BackUpFilesAsync();
            
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
    
    
    private async Task BackUpFilesAsync()
    {
        var tasks = new List<Task>();
        foreach (var file in FilesAndOutputDirectories)
        {
            try
            {
                tasks.Add(CopyFileAsync(file));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
    
            
        }
        await Task.WhenAll(tasks);
    }
    
    
    private async Task CopyFileAsync(KeyValuePair<string, string> file)
    {
        string sourceFile = file.Key;
        string outputDestination = file.Value;
        
        StringBuilder sb = new();
        sb.Append(Path.GetFileNameWithoutExtension(sourceFile));
        sb.Append(FormattedDateTimeNow);
        sb.Append(Path.GetExtension(sourceFile));
        
        await Task.Run(() => File.Copy(sourceFile, Path.Combine(outputDestination, sb.ToString())));
        Console.WriteLine($"File {Path.GetFileName(sourceFile)} copied successfully to directory {outputDestination}.");
    }
}