// <copyright file="ObservableRangeCollection{T}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace System.Collections.ObjectModel
{
    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.
    // See the LICENSE file in the project root for more information.
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Implementation of a dynamic data collection based on generic <see cref="Collection{T}"/>,
    /// implementing INotifyCollectionChanged to notify listeners
    /// when items get added, removed or the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [PropertyChanged.SuppressPropertyChangedWarnings]
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        [NonSerialized]
        private DeferredEventsCollection? deferredEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that is empty and has default initial capacity.
        /// </summary>
        public ObservableRangeCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that contains
        /// elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference.</exception>
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the <see cref="ObservableRangeCollection{T}"/> in the same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference.</exception>
        public ObservableRangeCollection(List<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void AddRange(IEnumerable<T> collection) => this.InsertRange(this.Count, collection);

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="ObservableCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="List{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not in the collection range.</exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    return;
                }
            }
            else if (!ContainsAny(collection))
            {
                return;
            }

            this.CheckReentrancy();

            // expand the following couple of lines when adding more constructors.
            var target = (List<T>)this.Items;
            target.InsertRange(index, collection);

            this.OnEssentialPropertiesChanged();

            if (!(collection is IList list))
            {
                list = new List<T>(collection);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list, index));
        }

        /// <summary>
        /// Removes the first occurence of each item in the specified collection from the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">The items to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (this.Count == 0)
            {
                return;
            }
            else if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    return;
                }
                else if (countable.Count == 1)
                {
                    using var enumerator = countable.GetEnumerator();
                    enumerator.MoveNext();
                    this.Remove(enumerator.Current);
                    return;
                }
            }
            else if (!ContainsAny(collection))
            {
                return;
            }

            this.CheckReentrancy();

            var clusters = new Dictionary<int, IList<T>>();
            var lastIndex = -1;
            IList<T>? lastCluster = null;
            foreach (var item in collection)
            {
                var index = this.IndexOf(item);
                if (index < 0)
                {
                    continue;
                }

                this.Items.RemoveAt(index);

                if (lastIndex == index && lastCluster != null)
                {
                    lastCluster.Add(item);
                }
                else
                {
                    clusters[lastIndex = index] = lastCluster = new List<T> { item };
                }
            }

            this.OnEssentialPropertiesChanged();

            if (this.Count == 0)
            {
                this.OnCollectionReset();
            }
            else
            {
                foreach (var cluster in clusters)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster.Value, cluster.Key));
                }
            }
        }

        /// <summary>
        /// Iterates over the collection and removes all items that satisfy the specified match.
        /// </summary>
        /// <remarks>The complexity is O(n).</remarks>
        /// <param name="match">The match.</param>
        /// <returns>Returns the number of elements that where.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/> null.</exception>
        public int RemoveAll(Predicate<T> match) => this.RemoveAll(0, this.Count, match);

        /// <summary>
        /// Iterates over the specified range within the collection and removes all items that satisfy the specified match.
        /// </summary>
        /// <remarks>The complexity is O(n).</remarks>
        /// <param name="index">The index of where to start performing the search.</param>
        /// <param name="count">The number of items to iterate on.</param>
        /// <param name="match">The match.</param>
        /// <returns>Returns the number of elements that where.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public int RemoveAll(int index, int count, Predicate<T> match)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (index + count > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (this.Count == 0)
            {
                return 0;
            }

            IList<T>? cluster = null;
            var clusterIndex = -1;
            var removedCount = 0;

            using (this.BlockReentrancy())
            {
                using (this.DeferEvents())
                {
                    for (var i = 0; i < count; i++, index++)
                    {
                        var item = this.Items[index];
                        if (match(item))
                        {
                            this.Items.RemoveAt(index);
                            removedCount++;

                            if (clusterIndex == index)
                            {
                                cluster!.Add(item);
                            }
                            else
                            {
                                cluster = new List<T> { item };
                                clusterIndex = index;
                            }

                            index--;
                        }
                        else if (clusterIndex > -1)
                        {
                            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
                            clusterIndex = -1;
                            cluster = null;
                        }
                    }

                    if (clusterIndex > -1)
                    {
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, cluster, clusterIndex));
                    }
                }
            }

            if (removedCount > 0)
            {
                this.OnEssentialPropertiesChanged();
            }

            return removedCount;
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="ObservableCollection{T}"/>>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified range is exceeding the collection.</exception>
        public void RemoveRange(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (index + count > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count == 0)
            {
                return;
            }

            if (count == 1)
            {
                this.RemoveItem(index);
                return;
            }

            // Items will always be List<T>, see constructors
            var items = (List<T>)this.Items;
            var removedItems = items.GetRange(index, count);

            this.CheckReentrancy();

            items.RemoveRange(index, count);

            this.OnEssentialPropertiesChanged();

            if (this.Count == 0)
            {
                this.OnCollectionReset();
            }
            else
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
            }
        }

        /// <summary>
        /// Clears the current collection and replaces it with the specified collection, using the default <see cref="EqualityComparer{T}"/>.
        /// </summary>
        /// <param name="collection">The items to fill the collection with, after clearing it.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void ReplaceRange(IEnumerable<T> collection) => this.ReplaceRange(0, this.Count, collection, EqualityComparer<T>.Default);

        /// <summary>
        /// Clears the current collection and replaces it with the specified collection,
        /// using the specified comparer to skip equal items.
        /// </summary>
        /// <param name="collection">The items to fill the collection with, after clearing it.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to be used
        /// to check whether an item in the same location already existed before,
        /// which in case it would not be added to the collection, and no event will be raised for it.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
        public void ReplaceRange(IEnumerable<T> collection, IEqualityComparer<T> comparer) => this.ReplaceRange(0, this.Count, collection, comparer);

        /// <summary>
        /// Removes the specified range and inserts the specified collection,
        /// ignoring equal items (using <see cref="EqualityComparer{T}.Default"/>).
        /// </summary>
        /// <param name="index">The index of where to start the replacement.</param>
        /// <param name="count">The number of items to be replaced.</param>
        /// <param name="collection">The collection to insert in that location.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void ReplaceRange(int index, int count, IEnumerable<T> collection) => this.ReplaceRange(index, count, collection, EqualityComparer<T>.Default);

        /// <summary>
        /// Removes the specified range and inserts the specified collection in its position, leaving equal items in equal positions intact.
        /// </summary>
        /// <param name="index">The index of where to start the replacement.</param>
        /// <param name="count">The number of items to be replaced.</param>
        /// <param name="collection">The collection to insert in that location.</param>
        /// <param name="comparer">The comparer to use when checking for equal items.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is out of range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
        public void ReplaceRange(int index, int count, IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (index + count > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    this.RemoveRange(index, count);
                    return;
                }
            }
            else if (!ContainsAny(collection))
            {
                this.RemoveRange(index, count);
                return;
            }

            if (index + count == 0)
            {
                this.InsertRange(0, collection);
                return;
            }

            if (!(collection is IList<T> list))
            {
                list = new List<T>(collection);
            }

            using (this.BlockReentrancy())
            using (this.DeferEvents())
            {
                var rangeCount = index + count;
                var addedCount = list.Count;

                var changesMade = false;
                IList<T>? newCluster = null;
                IList<T>? oldCluster = null;

                var i = index;
                for (; i < rangeCount && i - index < addedCount; i++)
                {
                    // parallel position
                    T old = this[i], @new = list[i - index];
                    if (comparer.Equals(old, @new))
                    {
                        this.OnRangeReplaced(i, newCluster!, oldCluster!);
                        continue;
                    }
                    else
                    {
                        this.Items[i] = @new;

                        if (newCluster is null || oldCluster is null)
                        {
                            newCluster = new List<T> { @new };
                            oldCluster = new List<T> { old };
                        }
                        else
                        {
                            newCluster.Add(@new);
                            oldCluster.Add(old);
                        }

                        changesMade = true;
                    }
                }

                this.OnRangeReplaced(i, newCluster!, oldCluster!);

                // exceeding position
                if (count != addedCount)
                {
                    var items = (List<T>)this.Items;
                    if (count > addedCount)
                    {
                        var removedCount = rangeCount - addedCount;
                        var removed = new T[removedCount];
                        items.CopyTo(i, removed, 0, removed.Length);
                        items.RemoveRange(i, removedCount);
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, i));
                    }
                    else
                    {
                        var k = i - index;
                        var added = new T[addedCount - k];
                        for (var j = k; j < addedCount; j++)
                        {
                            added[j - k] = list[j];
                        }

                        items.InsertRange(i, added);
                        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, added, i));
                    }

                    this.OnEssentialPropertiesChanged();
                }
                else if (changesMade)
                {
                    this.OnIndexerPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            if (this.Count == 0)
            {
                return;
            }

            this.CheckReentrancy();
            base.ClearItems();
            this.OnEssentialPropertiesChanged();
            this.OnCollectionReset();
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            if (Equals(this[index], item))
            {
                return;
            }

            this.CheckReentrancy();
            var originalItem = this[index];
            base.SetItem(index, item);

            this.OnIndexerPropertyChanged();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        /// <inheritdoc/>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.deferredEvents != null)
            {
                this.deferredEvents.Add(e);
                return;
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Defers the events.
        /// </summary>
        /// <returns>The disposable.</returns>
        protected virtual IDisposable DeferEvents() => new DeferredEventsCollection(this);

        private static bool ContainsAny(IEnumerable<T> collection)
        {
            using var enumerator = collection.GetEnumerator();
            return enumerator.MoveNext();
        }

        private void OnEssentialPropertiesChanged()
        {
            this.OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            this.OnIndexerPropertyChanged();
        }

        private void OnIndexerPropertyChanged() => this.OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object? oldItem, object? newItem, int index) => this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));

        private void OnCollectionReset() => this.OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

        private void OnRangeReplaced(int followingItemIndex, ICollection<T> newCluster, ICollection<T>? oldCluster)
        {
            if (oldCluster == null || oldCluster.Count == 0)
            {
                return;
            }

            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    new List<T>(newCluster),
                    new List<T>(oldCluster),
                    followingItemIndex - oldCluster.Count));

            oldCluster.Clear();
            newCluster.Clear();
        }

        private sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
        {
            private readonly ObservableRangeCollection<T> collection;

            public DeferredEventsCollection(ObservableRangeCollection<T> collection)
            {
                this.collection = collection;
                this.collection.deferredEvents = this;
            }

            public void Dispose()
            {
                this.collection.deferredEvents = null;
                foreach (var args in this)
                {
                    this.collection.OnCollectionChanged(args);
                }
            }
        }
    }
}
