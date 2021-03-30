using System.Collections.Concurrent;

namespace Chikatto.Extensions
{
    public static class Extensions
    {
        public static void Remove<K, V>(this ConcurrentDictionary<K, V> dict, K key) => dict.TryRemove(key, out _);
    }
}