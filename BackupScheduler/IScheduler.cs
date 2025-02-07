namespace BackupScheduler;

public interface IScheduler
{
    public double IntervalInMilliseconds { get; set; }

 

    void Start();
}