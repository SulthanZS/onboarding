
namespace OB_Net_Core_Tutorial.QuartzProject.Interface
{
    public interface ISchedulerService
    {
        void Initialize(string logtimeParam);
        void Start();
        void Stop();
    }
}
