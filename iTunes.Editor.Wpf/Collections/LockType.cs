// <copyright file="LockType.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Collections
{
    /// <summary>
    /// The lock type.
    /// </summary>
    public enum LockType
    {
        /// <summary>
        /// Spin/Wait lock type.
        /// </summary>
        SpinWait,

        /// <summary>
        /// Lock lock type.
        /// </summary>
        Lock,
    }
}
