// <copyright file="ShellPropertyDescription.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{
    using System;
    using System.Runtime.InteropServices;
    using MS.WindowsAPICodePack.Internal;

    /// <summary>Defines the shell property description information for a property.</summary>
    public class ShellPropertyDescription : IDisposable
    {
        private Type? valueType;
        private PropertyKey propertyKey;
        private IPropertyDescription? nativePropertyDescription;
        private VarEnum? varEnumType;
        private string? canonicalName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellPropertyDescription"/> class.
        /// </summary>
        /// <param name="key">The property key.</param>
        internal ShellPropertyDescription(PropertyKey key) => this.propertyKey = key;

        /// <summary>Gets the case-sensitive name of a property as it is known to the system, regardless of its localized name.</summary>
        public string CanonicalName
        {
            get
            {
                if (this.canonicalName is null)
                {
                    PropertySystemNativeMethods.PSGetNameFromPropertyKey(ref this.propertyKey, out this.canonicalName);
                }

                return this.canonicalName;
            }
        }

        /// <summary>Gets the .NET system type for a value of this property, or null if the value is empty.</summary>
        public Type ValueType => this.valueType
            ??= ShellPropertyFactory.VarEnumToSystemType(this.VarEnumType);

        /// <summary>Gets the VarEnum OLE type for this property.</summary>
        public VarEnum VarEnumType
        {
            get
            {
                if (this.NativePropertyDescription is not null && this.varEnumType is null)
                {
                    var hr = this.NativePropertyDescription.GetPropertyType(out var tempType);

                    if (CoreErrorHelper.Succeeded(hr))
                    {
                        this.varEnumType = tempType;
                    }
                }

                return this.varEnumType ?? default;
            }
        }

        /// <summary>Gets the native property description COM interface.</summary>
        internal IPropertyDescription NativePropertyDescription
        {
            get
            {
                if (this.nativePropertyDescription is null)
                {
                    var guid = new Guid(ShellIIDGuid.IPropertyDescription);
                    PropertySystemNativeMethods.PSGetPropertyDescription(ref this.propertyKey, ref guid, out this.nativePropertyDescription);
                }

                return this.nativePropertyDescription;
            }
        }

        /// <summary>Release the native objects.</summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Release the native objects.</summary>
        /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.nativePropertyDescription is not null)
            {
                Marshal.ReleaseComObject(this.nativePropertyDescription);
                this.nativePropertyDescription = null;
            }

            if (disposing)
            {
                // and the managed ones
                this.valueType = null;
            }
        }
    }
}
