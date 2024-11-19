using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCache
{
    public interface IDatabaseCacher
    {
        Task<T> GetOrSetAsync<T>(string cacheKey, Func<Task<T>> getItemFunction);
        Task<T> GetOrSetAsync<T>(string cacheKey, int timeoutminutes, Func<Task<T>> getItemFunction);

    }
}
