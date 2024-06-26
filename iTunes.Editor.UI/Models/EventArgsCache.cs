﻿// <copyright file="EventArgsCache.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace System.Collections.ObjectModel;

/// <summary>
/// To be kept outside <see cref="ObservableCollection{T}"/>, since otherwise, a new instance will be created for each generic type used.
/// </summary>
internal static class EventArgsCache
{
    /// <summary>
    /// The <see cref="ICollection.Count"/> <see cref="ComponentModel.PropertyChangedEventArgs"/>.
    /// </summary>
    internal static readonly ComponentModel.PropertyChangedEventArgs CountPropertyChanged = new(nameof(ICollection.Count));

    /// <summary>
    /// The index <see cref="ComponentModel.PropertyChangedEventArgs"/>.
    /// </summary>
    internal static readonly ComponentModel.PropertyChangedEventArgs IndexerPropertyChanged = new("Item[]");

    /// <summary>
    /// The reset collection <see cref="Specialized.NotifyCollectionChangedEventArgs"/>.
    /// </summary>
    internal static readonly Specialized.NotifyCollectionChangedEventArgs ResetCollectionChanged = new(Specialized.NotifyCollectionChangedAction.Reset);
}
