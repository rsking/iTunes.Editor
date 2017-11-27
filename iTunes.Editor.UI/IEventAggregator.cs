// -----------------------------------------------------------------------
// <copyright file="IEventAggregator.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// Represents an event aggregator.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets the event as an <see cref="System.IObservable{T}"/> sequence.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <returns>The event as an <see cref="System.IObservable{T}"/> sequence.</returns>
        System.IObservable<TEvent> GetEvent<TEvent>();

        /// <summary>
        /// Publishes the event.
        /// </summary>
        /// <typeparam name="TEvent">The event to publish.</typeparam>
        /// <param name="sampleEvent">The sample event.</param>
        void Publish<TEvent>(TEvent sampleEvent);
    }
}
