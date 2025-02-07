namespace BackupScheduler;

class Program
{
    
    
    static void Main(string[] args)
    {
        
        Console.WriteLine("Welcome to the Backup Scheduler.");
        string choice = ";";
        while (choice != "q" && choice!= "d" && choice != "f")
        {
                
            Console.WriteLine("Are you backing up individual files or directories? (Enter 'f' for files or 'd' for directories)");
            
            choice = Console.ReadLine() ?? string.Empty;
            choice = choice.ToLower();
        }
    
        IScheduler? scheduler = null;
        
        switch (choice)
        {
            case "f":
               scheduler = BackupFile();
                break;
            case "d":
                scheduler = BackupDirectory();
                break;
            default:
                Console.WriteLine("Invalid input. Please enter 'f' for files or 'd' for directories.");
                break;
        }
        
       
        scheduler?.Start();
        
      
        
       
    }

    private static IScheduler BackupDirectory()
    {
        var directory = GetDirectories();

        float interval = GetTimeInMinutes();
       
        return  new FolderScheduler(directory, interval * 60000);
    }

    private static IScheduler BackupFile()
    {
        var files = GetFiles();
        float interval = GetTimeInMinutes();

        return new FileSaveScheduler(files, interval * 60000);
       
    }

    private static Dictionary<string, string> GetFiles()
    {
        string key = "";
        Dictionary<string, string> files = new();
        Console.WriteLine("Enter the exact path to the file(s) you want to back up (EX: C:\\Users\\User\\Desktop\\file.txt), Type \"q\" when finished: ");
        while (true)
        {
            
            key = Console.ReadLine() ?? string.Empty;
            key = key.Replace("\"", "");
            key = key.Replace("\'", "");
            if(key.ToLower() == "q")
            {
                break;
            }
            
            if(!string.IsNullOrEmpty(key) && File.Exists(key))
            {

                var value = GetDirectory(key);
                files.Add(key, value); 
                Console.WriteLine(key + " Will be copied to " + value + ", Enter another file or type \"q\" to finish: ");
                continue;
            }
            Console.WriteLine("Invalid input. Please enter a valid file path.");
        
        }

        return files;
    }
    
    
    private static Dictionary<string, string> GetDirectories()
    {
        string key = "";
        Dictionary<string, string> files = new();
        Console.WriteLine("Enter the exact path to the directory you want to back up (EX: C:\\Users\\User\\Desktop), Type \"q\" when finished: ");
        while (true)
        {
            key = Console.ReadLine() ?? string.Empty;
            key = key.Replace("\"", "");
            key = key.Replace("\'", "");
            if(key.ToLower() == "q")
            {
                break;
            }
            if(!string.IsNullOrEmpty(key) && Directory.Exists(key))
            {

                var value = GetDirectory(key);
                files.Add(key, value); 
                Console.Write(key + " Will be copied to " + value + ", Enter another directory or type \"q\" to finish: ");
                continue;
            }
            Console.WriteLine("Invalid input. Please enter a valid directory.");
        }

        
        return files;
    }
    
    private static string GetDirectory(string key)
    {
        var value = "";
        Console.WriteLine("Enter the Output Directory for the file (EX: C:\\Users\\User\\Desktop\\): ");
        while (true)
        {
            
            value = value.Replace("\"", "");
            value = value.Replace("\'", "");
            value = Console.ReadLine() ?? string.Empty;
            if(value.ToLower() == "q")
            {
                Environment.Exit(1);
            }
            if(!string.IsNullOrEmpty(value) && Directory.Exists(value))
            {
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid directory.");
        }

        return value;
    }

    public static float GetTimeInMinutes()
    {
        
        Console.WriteLine("Enter the interval in minutes you want to back up the directory (EX: 5): ");
        string choice = "";
        while (true)
        {
            if (choice.ToLower() == "q")
            {
                Environment.Exit(1);
            }
            choice = Console.ReadLine() ?? string.Empty;
            if(float.TryParse(choice, out var num))
            {
                if (num >= 0.1f)
                {
                    return num;
                }
                else
                {
                    Console.WriteLine("Time must be greater than 0.1 minutes.");
                }
            }
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
       
    }


    

    
  
}