
namespace HW2.BackgroundTasks
{
    public interface IBackgroundTask
    {
        Task Start(CancellationToken ct);
    }
}
