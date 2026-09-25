using System.Threading.Tasks;

namespace Project_IT.Services.Interfaces
{
    public interface ISettingService
    {
        Task<T> GetSettingAsync<T>(string key, T defaultValue = default) where T : class, new();
        Task SaveSettingAsync<T>(string key, T value, string description = null) where T : class;
        void ClearCache(string key);
    }
}
