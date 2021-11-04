// -----------------------------------------------------------------------
// <copyright file="IConfigurator{T}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Represents a configurator.
    /// </summary>
    /// <typeparam name="T">The type of item to configure.</typeparam>
    public interface IConfigurator<in T>
    {
        /// <summary>
        /// Configures the item.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><see langword="true"/> if <paramref name="source"/> was successfully configured; otherwise <see langword="false"/>.</returns>
        bool Configure(T source);

        /// <summary>
        /// Configures the item.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><see langword="true"/> if <paramref name="source"/> was successfully configured; otherwise <see langword="false"/>.</returns>
        System.Threading.Tasks.ValueTask<bool> ConfigureAsync(T source);
    }
}
