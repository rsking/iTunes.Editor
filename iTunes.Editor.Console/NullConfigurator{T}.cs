// <copyright file="NullConfigurator{T}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor;

/// <summary>
/// A null configurator.
/// </summary>
/// <typeparam name="T">The type to configure.</typeparam>
internal sealed class NullConfigurator<T> : IConfigurator<T>
{
    /// <inheritdoc/>
    public bool Configure(T source) => true;

    /// <inheritdoc/>
    public ValueTask<bool> ConfigureAsync(T source) => new(result: true);
}
