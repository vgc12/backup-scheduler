using System.Diagnostics;
using System.IO.Compression;
using System.Timers;

namespace BackupScheduler;

public class FolderScheduler(Dictionary<string, string> save, double intervalInMilliseconds)
    : IScheduler
{
    public Dictionary<string, string> SaveAndOutputDirectories { get; set; } = save;


    public string FormattedDateTimeNow => $"{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";

    public double IntervalInMilliseconds { get; set; } = intervalInMilliseconds;

    public void Start()
    {
        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Elapsed += OnTimedEvent;
        aTimer.Interval = IntervalInMilliseconds;
        aTimer.Enabled = true;
  
        BackUpDirectoryAsync();
         
        
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

        /*
        
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
              
                }
            }));
        }
        */
        
        
        
        foreach (var item in SaveAndOutputDirectories)
        {
            var startPath = item.Key;
            var zipPath = Path.Combine(item.Value, Path.GetFileName(Path.GetDirectoryName(item.Key)) + "_" + FormattedDateTimeNow + ".zip");
            
            tasks.Add(Task.Run(() =>
            {
              
                    using (var zipToOpen = new FileStream(zipPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                        {
                            var files = Directory.GetFiles(startPath, "*.*", SearchOption.AllDirectories);
                            for (var index = 0; index < files.Length; index++)
                            {
                                Console.WriteLine("(" + (index +1)+ "/" + files.Length + ")----------" + "Backing up file: " + files[index]);
                                var file = files[index];
                                while (!IsFileReady(file))
                                {
                                    
                                    Console.WriteLine($"{file} is not ready. Waiting for file to be ready...");
                                }

                                var relativePath = file[startPath.Length..];
                                archive.CreateEntryFromFile(file, relativePath);
                            }
                        }
                    }
                    Console.WriteLine($"Directory {startPath} backed up to {zipPath}");
                
               
            }));
        }
        

        await Task.WhenAll(tasks); 
    }



    public static bool IsFileReady(string filename)
    {
        // If the file can be opened for exclusive access it means that the file
        // is no longer locked by another process.
        try
        {
            using FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            if(inputStream.Length > 0)
            {
                inputStream.Close();
                return true;
            }
            else
            {
                return false;
            }
            
        }
        catch (Exception)
        {
            return false;
        }
    }
}

