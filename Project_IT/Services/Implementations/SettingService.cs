using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using Newtonsoft.Json;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Services.Implementations
{
    public class SettingService : ISettingService
    {
        private readonly ApplicationDbContext _db;

        public SettingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<T> GetSettingAsync<T>(string key, T defaultValue = default) where T : class, new()
        {
            string cacheKey = $"sys_setting_{key}";

            // 1. Try reading from HttpRuntime.Cache
            if (HttpRuntime.Cache != null && HttpRuntime.Cache[cacheKey] is T cachedValue)
            {
                return cachedValue;
            }

            // 2. Fetch from Database if missing from cache
            var setting = await _db.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null || string.IsNullOrWhiteSpace(setting.Value))
            {
                return defaultValue ?? new T();
            }

            // 3. Deserialize JSON to strongly typed object
            T result = JsonConvert.DeserializeObject<T>(setting.Value) ?? (defaultValue ?? new T());

            // 4. Store in Cache with 2-Hour Sliding Expiration & Low Memory Priority
            if (HttpRuntime.Cache != null)
            {
                HttpRuntime.Cache.Insert(
                    key: cacheKey,
                    value: result,
                    dependencies: null,
                    absoluteExpiration: Cache.NoAbsoluteExpiration,
                    slidingExpiration: TimeSpan.FromHours(2),
                    priority: CacheItemPriority.BelowNormal, // Purged first by IIS if system RAM gets low
                    onRemoveCallback: null
                );
            }

            return result;
        }

        public async Task SaveSettingAsync<T>(string key, T value, string description = null) where T : class
        {
            string jsonValue = JsonConvert.SerializeObject(value);

            var setting = await _db.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting == null)
            {
                setting = new SystemSetting
                {
                    Key = key,
                    Value = jsonValue,
                    Description = description,
                    LastModified = DateTime.UtcNow
                };
                _db.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = jsonValue;
                setting.LastModified = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(description))
                {
                    setting.Description = description;
                }
            }

            await _db.SaveChangesAsync();

            // Evict cache so public visitors see updated settings immediately
            ClearCache(key);
        }

        public void ClearCache(string key)
        {
            string cacheKey = $"sys_setting_{key}";
            if (HttpRuntime.Cache != null)
            {
                HttpRuntime.Cache.Remove(cacheKey);
            }
        }
    }
}
