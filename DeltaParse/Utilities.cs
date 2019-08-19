using System;
using System.Collections.Generic;

namespace DeltaParse
{
    public static class Utilities
    {
        public static int SubstringCount(string text, string pattern)
        {
            return (text.Length - text.Replace(pattern, string.Empty).Length) / pattern.Length;
        }

        public static void AddToDictionaryCollection<TKey, TValueCollection, TValue>(IDictionary<TKey, TValueCollection> dictionary, TKey key, TValue value)
            where TValueCollection : ICollection<TValue>, new()
        {
            if (dictionary.TryGetValue(key, out var valueList))
            {
                valueList.Add(value);
            }
            else
            {
                dictionary[key] = new TValueCollection { value };
            }
        }
    }
}
