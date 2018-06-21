// -----------------------------------------------------------------------
// <copyright file="EventAggregator.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// An <see cref="IEventAggregator"/> based on <see cref="System.Reactive.Linq.Observable"/> instances.
    /// </summary>
    public class EventAggregator : IEventAggregator, System.IDisposable
    {
        /// <summary>
        /// The subject.
        /// </summary>
        private readonly System.Reactive.Subjects.Subject<object> subject = new System.Reactive.Subjects.Subject<object>();

        /// <summary>
        /// Whether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the event as an <see cref="System.IObservable{T}"/> sequence.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <returns>The event as an <see cref="System.IObservable{T}"/> sequence.</returns>
        public System.IObservable<TEvent> GetEvent<TEvent>() => System.Reactive.Linq.Observable.AsObservable(System.Reactive.Linq.Observable.OfType<TEvent>(this.subject));

        /// <summary>
        /// Publishes the event.
        /// </summary>
        /// <typeparam name="TEvent">The event to publish.</typeparam>
        /// <param name="sampleEvent">The sample event.</param>
        public void Publish<TEvent>(TEvent sampleEvent) => this.subject.OnNext(sampleEvent);

        /// <summary>
        /// Disposes unmanaged, and optionally managed, resources used by this <see cref="IEventAggregator"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.subject.Dispose();
            this.disposed = true;
        }
    }
}
