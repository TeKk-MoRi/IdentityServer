using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IdentityServer.Traditional.Helper.Redis
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null,
            bool expire = false)
        {
            var options = new DistributedCacheEntryOptions();
            if (expire)
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1);
                options.SlidingExpiration = unusedExpireTime;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(120);
                options.SlidingExpiration = unusedExpireTime;
            }


            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache
            , string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
