using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Blazored.Typeahead.DynamicComponent
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged
    {
        #region Constructors

        public ObservableDictionary() { }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }
        public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : base(collection, comparer) { }
        public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public ObservableDictionary(int capacity) : base(capacity) { }
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base (capacity, comparer) { }

        #endregion

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnPropertyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey,TValue>(key, value)));
        }

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                OnPropertyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
            }
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            var result = base.TryAdd(key, value);
            if (result)
            {
                OnPropertyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
            }

            return result;
        }

        public new void Clear()
        {
            base.Clear();
            OnPropertyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, this));
        }

        public new bool Remove(TKey key)
        {
            var result = base.Remove(key);

            if (result)
            {
                OnPropertyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, key));
            }

            return result;
        }

        public new bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            var result = base.Remove(key, out value);

            if (result)
            {
                OnPropertyChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            }

            return result;
        }

        protected virtual void OnPropertyChanged(NotifyCollectionChangedEventArgs collectionEventArgs)
        {
            CollectionChanged?.Invoke(this, collectionEventArgs);
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
    }
}
