// <copyright file="ObservableCollectionObject.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Collections;

using System.Collections.Specialized;
using System.ComponentModel;

using System.Runtime.CompilerServices;

using System.Windows.Threading;

/// <summary>
/// Observable collection object.
/// </summary>
public abstract class ObservableCollectionObject : INotifyCollectionChanged, INotifyPropertyChanged
{
    private readonly object lockObj = new();

    private bool lockObjWasTaken;

    private int @lock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollectionObject"/> class.
    /// </summary>
    /// <param name="lockType">The lock type.</param>
    protected ObservableCollectionObject(LockType lockType) => this.LockType = lockType;

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the lock type.
    /// </summary>
    public LockType LockType { get; }

    /// <summary>
    /// Do events.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DoEvents()
    {
        if (GetDispatcher() is Dispatcher dispatcher)
        {
            var frame = new DispatcherFrame();
            _ = dispatcher.BeginInvoke(DispatcherPriority.DataBind, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public void RaisePropertyChanged(string propertyName)
    {
        var propertyChangedEventHandler = this.PropertyChanged;

        if (propertyChangedEventHandler is not null)
        {
            propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Pumps until the condition.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="condition">The condition.</param>
    protected static void PumpWaitAndPumpUntil(Dispatcher dispatcher, Func<bool> condition)
    {
        var frame = new DispatcherFrame();
        BeginInvokePump(dispatcher, frame, condition);
        Dispatcher.PushFrame(frame);
    }

    /// <summary>
    /// returns a valid dispatcher if this is a UI thread (can be more than one UI thread so different dispatchers are possible); null if not a UI thread.
    /// </summary>
    /// <returns>The dispatcher thread.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static Dispatcher GetDispatcher() => Dispatcher.FromThread(Thread.CurrentThread);

    /// <summary>
    /// Waits for the condition.
    /// </summary>
    /// <param name="condition">The condition.</param>
    protected void WaitForCondition(Func<bool> condition)
    {
        if (GetDispatcher() is Dispatcher dispatcher)
        {
            this.lockObjWasTaken = true;
            PumpWaitAndPumpUntil(dispatcher, condition);
            return;
        }

        switch (this.LockType)
        {
            case LockType.SpinWait:
                SpinWait.SpinUntil(condition); // spin baby...
                break;
            case LockType.Lock:
                var isLockTaken = false;
                Monitor.Enter(this.lockObj, ref isLockTaken);
                this.lockObjWasTaken = isLockTaken;
                break;
        }
    }

    /// <summary>
    /// Tries to get a lock.
    /// </summary>
    /// <returns><see langword="true"/> if a lock was obtained.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool TryLock() => this.LockType switch
    {
        LockType.SpinWait => Interlocked.CompareExchange(ref this.@lock, 1, 0) == 0,
        LockType.Lock => Monitor.TryEnter(this.lockObj),
        _ => false,
    };

    /// <summary>
    /// Locks this instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void Lock()
    {
        Func<bool>? action = this.LockType switch
        {
            LockType.SpinWait => () => Interlocked.CompareExchange(ref this.@lock, 1, 0) == 0,
            LockType.Lock => () => Monitor.TryEnter(this.lockObj),
            _ => default,
        };

        if (action is not null)
        {
            this.WaitForCondition(action);
        }
    }

    /// <summary>
    /// Unlocks this instance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void Unlock()
    {
        switch (this.LockType)
        {
            case LockType.SpinWait:
                this.@lock = 0;
                break;
            case LockType.Lock:
                if (this.lockObjWasTaken)
                {
                    Monitor.Exit(this.lockObj);
                }

                this.lockObjWasTaken = false;
                break;
        }
    }

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        var notifyCollectionChangedEventHandler = this.CollectionChanged;

        if (notifyCollectionChangedEventHandler is null)
        {
            return;
        }

        foreach (var handler in notifyCollectionChangedEventHandler
            .GetInvocationList()
            .OfType<NotifyCollectionChangedEventHandler>())
        {
            if (handler.Target is DispatcherObject dispatcherObject && !dispatcherObject.CheckAccess())
            {
                _ = dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
            }
            else
            {
                handler(this, args);
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event.
    /// </summary>
    protected virtual void RaiseNotifyCollectionChanged() => this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    /// <summary>
    /// Raises the <see cref="CollectionChanged"/> event.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    protected virtual void RaiseNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        this.RaisePropertyChanged("Count");
        this.RaisePropertyChanged("Item[]");
        this.OnCollectionChanged(args);
    }

    private static void BeginInvokePump(Dispatcher dispatcher, DispatcherFrame frame, Func<bool> condition)
    {
        var action = new Action(() =>
        {
            frame.Continue = !condition();

            if (frame.Continue)
            {
                BeginInvokePump(dispatcher, frame, condition);
            }
        });

        _ = dispatcher.BeginInvoke(DispatcherPriority.DataBind, action);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object? ExitFrame(object frame)
    {
        ((DispatcherFrame)frame).Continue = false;
        return null;
    }
}
