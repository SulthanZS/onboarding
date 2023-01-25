using System.Threading.Tasks;

namespace OB_Net_Core_Tutorial.BLL.Interface
{
    public interface IRedisService
    {
        Task SaveAsync(string key, object value);
        Task<T> GetAsync<T>(string key);

        T GetValue<T>(string key);
        Task<bool> DeleteAsync(string key);
    }
}
