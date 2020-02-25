namespace System.Collections.ObjectModel
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    public class ObservableSortableCollection<T> : ObservableRangeCollection<T>
    {
        private IComparer<T> comparer;

        private bool isDescending;

        private bool reordering;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortableCollection{T}"/> class.
        /// </summary>
        public ObservableSortableCollection() => this.comparer = Comparer<T>.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortableCollection{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="descending">Set to <see langword="true"/> to sort in descending order.</param>
        public ObservableSortableCollection(IComparer<T> comparer, bool descending = false)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            this.IsDescending = descending;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortableCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The base collection.</param>
        /// <param name="comparer">The comparer.</param>
        /// <param name="descending">Set to <see langword="true"/> to sort in descending order.</param>
        public ObservableSortableCollection(IEnumerable<T> collection, IComparer<T> comparer, bool descending = false)
            : base(collection)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            this.IsDescending = descending;

            this.Sort();
        }

        /// <summary>
        /// Gets or sets the comparer.
        /// </summary>
        public IComparer<T> Comparer
        {
            get => this.comparer;
            set
            {
                this.comparer = value ?? Comparer<T>.Default;
                this.Sort();
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Comparer)));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sorting should be descending.
        /// Default value is false.
        /// </summary>
        public bool IsDescending
        {
            get => this.isDescending;
            set
            {
                if (this.isDescending != value)
                {
                    this.isDescending = value;
                    this.Sort();
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.IsDescending)));
                }
            }
        }

        /// <summary>
        /// Sorts the list.
        /// </summary>
        public void Sort() // TODO, concern change index so no need to walk the whole list
        {
            var query = this
              .Select((item, index) => (Item: item, Index: index));
            query = this.IsDescending
              ? query.OrderByDescending(tuple => tuple.Item, this.Comparer)
              : query.OrderBy(tuple => tuple.Item, this.Comparer);

            var map = query.Select((tuple, index) => (OldIndex: tuple.Index, NewIndex: index))
             .Where(o => o.OldIndex != o.NewIndex);

            using var enumerator = map.GetEnumerator();
            if (enumerator.MoveNext())
            {
                this.reordering = true;
                this.Move(enumerator.Current.OldIndex, enumerator.Current.NewIndex);
                this.reordering = false;
            }
        }

        /// <inheritdoc/>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (this.reordering)
            {
                return;
            }

            if (e != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        return;
                }
            }

            this.Sort();
        }
    }
}
