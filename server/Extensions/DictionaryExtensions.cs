using System.Collections.Generic;

namespace PhotoAlbum
{
    public static class DictionaryExtensions
    {
        public static TVal SafeGet<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key) where TVal : class
        {
            return dict.ContainsKey(key)
                ? dict[key]
                : null;
        }
    }
}