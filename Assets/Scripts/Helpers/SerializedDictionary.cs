using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//* Taken from Temperament *//

[Serializable]
public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    [Serializable]
    public struct Entry
    {
        public TKey key;
        public TValue value;
    }

    public SerializableDictionary()
    {
        entries = new();
        dict = null;
    }

    [SerializeField]
    private List<Entry> entries;

    private Dictionary<TKey, TValue> dict = null;
    private Dictionary<TKey, TValue> Dictionary
    {
        get
        {
            if (dict is null)
            {
                dict = new Dictionary<TKey, TValue>();

                foreach (var entry in entries)
                    dict[entry.key] = entry.value;
            }

            return dict;
        }
    }

    public void Save()
    {
        entries.Clear();

        foreach(var kvp in Dictionary)
            entries.Add(new() { key = kvp.Key, value = kvp.Value });
    }

    public ICollection<TKey> Keys => Dictionary.Keys;
    public ICollection<TValue> Values => Dictionary.Values;

    public int Count => Dictionary.Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).IsReadOnly;

    public TValue this[TKey key] { get => Dictionary[key]; set => Dictionary[key] = value; }

    public void Add(TKey key, TValue value)
        => Dictionary.Add(key, value);

    public bool ContainsKey(TKey key)
        => Dictionary.ContainsKey(key);
    public bool Remove(TKey key)
        => Dictionary.Remove(key);

    public bool TryGetValue(TKey key, out TValue value)
        => Dictionary.TryGetValue(key, out value);

    public void Add(KeyValuePair<TKey, TValue> item)
        => Dictionary.Add(item.Key, item.Value);

    public void Clear()
        => Dictionary.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item)
        => (Dictionary as ICollection<KeyValuePair<TKey, TValue>>).Contains(item);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        => (Dictionary as ICollection<KeyValuePair<TKey, TValue>>).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<TKey, TValue> item)
        => (Dictionary as IDictionary<TKey, TValue>).Remove(item);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => Dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Dictionary.GetEnumerator();
}
