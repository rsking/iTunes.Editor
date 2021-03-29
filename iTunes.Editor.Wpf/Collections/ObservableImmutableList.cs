// <copyright file="ObservableImmutableList.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The observable immutable list.
    /// </summary>
    /// <typeparam name="T">The type in the list.</typeparam>
    public class ObservableImmutableList<T> : ObservableCollectionObject, IList, IList<T>, IImmutableList<T>
    {
        private ImmutableList<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableImmutableList{T}"/> class.
        /// </summary>
        public ObservableImmutableList()
            : this(Array.Empty<T>(), LockType.SpinWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableImmutableList{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public ObservableImmutableList(IEnumerable<T> items)
            : this(items, LockType.SpinWait)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableImmutableList{T}"/> class.
        /// </summary>
        /// <param name="lockType">The lock type.</param>
        public ObservableImmutableList(LockType lockType)
            : this(Array.Empty<T>(), lockType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableImmutableList{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="lockType">The lock type.</param>
        public ObservableImmutableList(IEnumerable<T> items, LockType lockType)
            : base(lockType)
        {
            this.SyncRoot = new object();
            this.items = ImmutableList<T>.Empty.AddRange(items);
        }

        /// <inheritdoc/>
        public bool IsFixedSize => false;

        /// <inheritdoc/>
        public int Count => this.items.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public bool IsSynchronized => false;

        /// <inheritdoc/>
        public object SyncRoot { get; }

        /// <inheritdoc/>
        object? IList.this[int index]
        {
            get => this[index];
            set => this.SetItem(index, (T)value!);
        }

        /// <inheritdoc/>
        public T this[int index]
        {
            get => this.items[index];
            set => this.SetItem(index, value);
        }

        /// <summary>
        /// Tries the operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryOperation(Func<ImmutableList<T>, ImmutableList<T>> operation) => this.TryOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        /// <summary>
        /// Does the operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DoOperation(Func<ImmutableList<T>, ImmutableList<T>> operation) => this.DoOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        /// <summary>
        /// Does the insert.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool DoInsert(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.DoOperation(currentItems =>
        {
            var kvp = valueProvider(currentItems);
            var newItems = currentItems.Insert(kvp.Key, kvp.Value);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp.Value, kvp.Key));
        });

        /// <summary>
        /// Does the add.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool DoAdd(Func<ImmutableList<T>, T> valueProvider) => this.DoOperation(currentItems =>
        {
            var value = valueProvider(currentItems);
            var newItems = this.items.Add(value);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, currentItems.Count));
        });

        /// <summary>
        /// Does the add range.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool DoAddRange(Func<ImmutableList<T>, IEnumerable<T>> valueProvider) => this.DoOperation(currentItems => currentItems.AddRange(valueProvider(currentItems)));

        /// <summary>
        /// Does the remove.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool DoRemove(Func<ImmutableList<T>, T> valueProvider) => this.DoRemoveAt(currentItems => currentItems.IndexOf(valueProvider(currentItems)));

        /// <summary>
        /// Does the remove at.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool DoRemoveAt(Func<ImmutableList<T>, int> valueProvider) => this.DoOperation(currentItems =>
        {
            var index = valueProvider(currentItems);
            var value = currentItems[index];
            var newItems = currentItems.RemoveAt(index);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        });

        /// <summary>
        /// Does the set item.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool DoSetItem(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.DoOperation(currentItems =>
        {
            var kvp = valueProvider(currentItems);
            var newValue = kvp.Value;
            var index = kvp.Key;
            var oldValue = currentItems[index];
            var newItems = currentItems.SetItem(kvp.Key, newValue);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue, index));
        });

        /// <summary>
        /// Tries the insert.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool TryInsert(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.TryOperation(currentItems =>
        {
            var kvp = valueProvider(currentItems);
            var newItems = currentItems.Insert(kvp.Key, kvp.Value);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp.Value, kvp.Key));
        });

        /// <summary>
        /// Tries the add.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool TryAdd(Func<ImmutableList<T>, T> valueProvider) => this.TryOperation(currentItems =>
        {
            var value = valueProvider(currentItems);
            var newItems = this.items.Add(value);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, currentItems.Count));
        });

        /// <summary>
        /// Tries the add range.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool TryAddRange(Func<ImmutableList<T>, IEnumerable<T>> valueProvider) => this.TryOperation(currentItems => currentItems.AddRange(valueProvider(currentItems)));

        /// <summary>
        /// Tries the remove.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool TryRemove(Func<ImmutableList<T>, T> valueProvider) => this.TryRemoveAt(currentItems => currentItems.IndexOf(valueProvider(currentItems)));

        /// <summary>
        /// Tries the remove at.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool TryRemoveAt(Func<ImmutableList<T>, int> valueProvider) => this.TryOperation(currentItems =>
        {
            var index = valueProvider(currentItems);
            var value = currentItems[index];
            var newItems = currentItems.RemoveAt(index);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        });

        /// <summary>
        /// Tries the set item.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>The operation result.</returns>
        public bool TrySetItem(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.TryOperation(currentItems =>
        {
            var kvp = valueProvider(currentItems);
            var newValue = kvp.Value;
            var index = kvp.Key;
            var oldValue = currentItems[index];
            var newItems = currentItems.SetItem(kvp.Key, newValue);
            return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue, index));
        });

        /// <summary>
        /// Converts this instance to an immutable list.
        /// </summary>
        /// <returns>The immutable list.</returns>
        public ImmutableList<T> ToImmutableList() => this.items;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        public int Add(object? value)
        {
            var val = (T)value!;
            this.Add(val);
            return this.IndexOf(val);
        }

        /// <inheritdoc/>
        public bool Contains(object? value) => this.Contains((T)value!);

        /// <inheritdoc/>
        public int IndexOf(object? value) => this.IndexOf((T)value!);

        /// <inheritdoc/>
        public void Insert(int index, object? value) => this.Insert(index, (T)value!);

        /// <inheritdoc/>
        public void Remove(object? value) => this.Remove((T)value!);

        /// <inheritdoc/>
        void IList.RemoveAt(int index) => this.RemoveAt(index);

        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => this.items.ToArray().CopyTo(array, index);

        /// <inheritdoc/>
        public int IndexOf(T item) => this.items.IndexOf(item);

        /// <inheritdoc/>
        void IList<T>.Insert(int index, T item) => this.Insert(index, item);

        /// <inheritdoc/>
        void IList<T>.RemoveAt(int index) => this.RemoveAt(index);

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => this.Add(item);

        /// <inheritdoc/>
        void IList.Clear() => this.Clear();

        /// <inheritdoc/>
        void ICollection<T>.Clear() => this.Clear();

        /// <inheritdoc/>
        public bool Contains(T item) => this.items.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public bool Remove([System.Diagnostics.CodeAnalysis.MaybeNull] T item)
        {
            if (!this.items.Contains(item))
            {
                return false;
            }

            this.items = this.items.Remove(item);
            this.RaiseNotifyCollectionChanged();
            return true;
        }

        /// <inheritdoc/>
        public IImmutableList<T> Add(T value)
        {
            var index = this.items.Count;
            this.items = this.items.Add(value);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> AddRange(IEnumerable<T> items)
        {
            this.items = this.items.AddRange(items);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> Clear()
        {
            this.items = this.items.Clear();
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public int IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) => this.items.IndexOf(item, index, count, equalityComparer);

        /// <inheritdoc/>
        public IImmutableList<T> Insert(int index, [System.Diagnostics.CodeAnalysis.MaybeNull] T element)
        {
            this.items = this.items.Insert(index, element);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, index));
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> InsertRange(int index, IEnumerable<T> items)
        {
            this.items = this.items.InsertRange(index, items);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public int LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) => this.items.LastIndexOf(item, index, count, equalityComparer);

        /// <inheritdoc/>
        public IImmutableList<T> Remove(T value, IEqualityComparer<T>? equalityComparer)
        {
            var index = this.items.IndexOf(value, equalityComparer);
            this.RemoveAt(index);
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveAll(Predicate<T> match)
        {
            this.items = this.items.RemoveAll(match);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveAt(int index)
        {
            var value = this.items[index];
            this.items = this.items.RemoveAt(index);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveRange(int index, int count)
        {
            this.items = this.items.RemoveRange(index, count);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer)
        {
            this.items = this.items.RemoveRange(items, equalityComparer);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer)
        {
            var index = this.items.IndexOf(oldValue, equalityComparer);
            this.SetItem(index, newValue);
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> SetItem(int index, [System.Diagnostics.CodeAnalysis.MaybeNull] T value)
        {
            var oldItem = this.items[index];
            this.items = this.items.SetItem(index, value);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableList<T>, ImmutableList<T>> operation, NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (this.TryLock())
                {
                    var oldList = this.items;
                    var newItems = operation(oldList);

                    if (newItems is null)
                    {
                        // user returned null which means he cancelled operation
                        return false;
                    }

                    this.items = newItems;

                    if (args is not null)
                    {
                        this.RaiseNotifyCollectionChanged(args);
                    }

                    return true;
                }
            }
            finally
            {
                this.Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableList<T>, KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>> operation)
        {
            try
            {
                if (this.TryLock())
                {
                    var oldList = this.items;
                    var kvp = operation(oldList);
                    var newItems = kvp.Key;
                    var args = kvp.Value;

                    if (newItems is null)
                    {
                        // user returned null which means he cancelled operation
                        return false;
                    }

                    this.items = newItems;

                    if (args is not null)
                    {
                        this.RaiseNotifyCollectionChanged(args);
                    }

                    return true;
                }
            }
            finally
            {
                this.Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableList<T>, ImmutableList<T>> operation, NotifyCollectionChangedEventArgs args)
        {
            bool result;

            try
            {
                this.Lock();
                var oldItems = this.items;
                var newItems = operation(this.items);

                if (newItems is null)
                {
                    // user returned null which means he cancelled operation
                    return false;
                }

                this.items = newItems;
                result = newItems != oldItems;

                if (args is not null)
                {
                    this.RaiseNotifyCollectionChanged(args);
                }
            }
            finally
            {
                this.Unlock();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableList<T>, KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>> operation)
        {
            bool result;

            try
            {
                this.Lock();
                var oldItems = this.items;
                var kvp = operation(this.items);
                var newItems = kvp.Key;
                var args = kvp.Value;

                if (newItems is null)
                {
                    // user returned null which means he cancelled operation
                    return false;
                }

                this.items = newItems;
                result = this.items != oldItems;

                if (args is not null)
                {
                    this.RaiseNotifyCollectionChanged(args);
                }
            }
            finally
            {
                this.Unlock();
            }

            return result;
        }
    }
}
