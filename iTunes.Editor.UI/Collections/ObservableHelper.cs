// <copyright file="ObservableHelper.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Collections;

using System.Collections;

/// <summary>
/// Helper class for observable collections.
/// </summary>
public static class ObservableHelper
{
    private static Func<Type, ICollection> observableCollectionFactory = CreateObservableCollectionFactory(typeof(System.Collections.ObjectModel.ObservableCollection<>));

    private static Func<Type, IList> observableListFactory = CreateObservableListFactory(typeof(System.Collections.ObjectModel.ObservableCollection<>));

    /// <summary>
    /// Creates an observable collection.
    /// </summary>
    /// <typeparam name="T">The type in the collection.</typeparam>
    /// <returns>The observable collection.</returns>
    public static ICollection<T> CreateObservableCollection<T>() => (ICollection<T>)observableCollectionFactory(typeof(T));

    /// <summary>
    /// Creates an observable list.
    /// </summary>
    /// <typeparam name="T">The type in the collection.</typeparam>
    /// <returns>The observable list.</returns>
    public static IList<T> CreateObservableList<T>() => (IList<T>)observableListFactory(typeof(T));

    /// <summary>
    /// Sets the observable collection factory.
    /// </summary>
    /// <param name="func">The factory.</param>
    public static void SetObservableCollectionFactory(Func<Type, ICollection> func) => observableCollectionFactory = func;

    /// <summary>
    /// Sets the observable collection to the specified type.
    /// </summary>
    /// <param name="collectionType">The collection type.</param>
    public static void SetObservableCollectionType(Type collectionType) => SetObservableCollectionFactory(CreateObservableCollectionFactory(collectionType));

    /// <summary>
    /// Sets the observable list factory.
    /// </summary>
    /// <param name="func">The factory.</param>
    public static void SetObservableListFactory(Func<Type, IList> func) => observableListFactory = func;

    /// <summary>
    /// Sets the observable collection to the specified type.
    /// </summary>
    /// <param name="listType">The list type.</param>
    public static void SetObservableListType(Type listType) => SetObservableListFactory(CreateObservableListFactory(listType));

    private static Func<Type, ICollection> CreateObservableCollectionFactory(Type collectionType) => !HasInterface(collectionType, typeof(ICollection<>)) || !typeof(System.Collections.Specialized.INotifyCollectionChanged).IsAssignableFrom(collectionType)
        ? throw new ArgumentException("Type must be a generic collection and implement INotifyCollectionChanged", nameof(collectionType))
        : (type => (ICollection)Activator.CreateInstance(collectionType.MakeGenericType(type)));

    private static Func<Type, IList> CreateObservableListFactory(Type listType) => !HasInterface(listType, typeof(IList<>)) || !typeof(System.Collections.Specialized.INotifyCollectionChanged).IsAssignableFrom(listType)
        ? throw new ArgumentException("Type must be a generic collection and implement INotifyCollectionChanged", nameof(listType))
        : (type => (IList)Activator.CreateInstance(listType.MakeGenericType(type)));

    private static bool HasInterface(Type type, Type interfaceType) => Array.Exists(type.GetInterfaces(), t => string.Equals(t.Name, interfaceType.Name, StringComparison.Ordinal));
}
