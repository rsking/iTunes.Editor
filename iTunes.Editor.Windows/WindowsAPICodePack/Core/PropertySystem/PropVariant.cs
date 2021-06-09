// <copyright file="PropVariant.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace MS.WindowsAPICodePack.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using Microsoft.WindowsAPICodePack.Resources;
    using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

#pragma warning disable CS0618, RCS1130, S3265
    /// <summary>Represents the OLE struct PROPVARIANT. This class is intended for internal use only.</summary>
    /// <remarks>
    /// Originally sourced from http://blogs.msdn.com/adamroot/pages/interop-with-propvariants-in-net.aspx and modified to support additional
    /// types including vectors and ability to set values.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public sealed class PropVariant : IDisposable
    {
        // A dictionary and lock to contain compiled expression trees for constructors
        private static readonly Dictionary<Type, Func<object, PropVariant>> Cache = new();

        private static readonly object Padlock = new();

        // A static dictionary of delegates to get data from array's contained within PropVariants
        private static Dictionary<Type, Action<PropVariant, Array, uint>>? vectorActions;

        [FieldOffset(8)]
        private readonly Blob blob;

        [FieldOffset(8)]
        private readonly int int32;

        [FieldOffset(8)]
        private readonly uint uint32;

        [FieldOffset(8)]
        private readonly byte @byte;

        [FieldOffset(8)]
        private readonly sbyte @sbyte;

        [FieldOffset(8)]
        private readonly short @short;

        [FieldOffset(8)]
        private readonly ushort @ushort;

        [FieldOffset(8)]
        private readonly long @long;

        [FieldOffset(8)]
        private readonly ulong @ulong;

        [FieldOffset(8)]
        private readonly double @double;

        [FieldOffset(8)]
        private readonly float @float;

        [FieldOffset(0)]
        private readonly decimal @decimal;

        [FieldOffset(8)]
        private IntPtr ptr;

        // This is actually a VarEnum value, but the VarEnum type requires 4 bytes instead of the expected 2.
        [FieldOffset(0)]
        private ushort valueType;

        //// Reserved Fields
        ////[FieldOffset(2)]
        //// ushort _wReserved1;
        //// [FieldOffset(4)]
        //// ushort _wReserved2;
        //// [FieldOffset(6)]
        //// ushort _wReserved3;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        public PropVariant()
        {
            // left empty
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The string.</param>
        public PropVariant(string value)
        {
            if (value is null)
            {
                throw new ArgumentException(LocalizedMessages.PropVariantNullString, nameof(value));
            }

            this.valueType = (ushort)VarEnum.VT_LPWSTR;
            this.ptr = Marshal.StringToCoTaskMemUni(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The string vector.</param>
        public PropVariant(string[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromStringVector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The bool vector.</param>
        public PropVariant(bool[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromBooleanVector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The short vector.</param>
        public PropVariant(short[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromInt16Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The ushort vector.</param>
        public PropVariant(ushort[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromUInt16Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The int vector.</param>
        public PropVariant(int[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromInt32Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The uint vector.</param>
        public PropVariant(uint[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromUInt32Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The long vector.</param>
        public PropVariant(long[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromInt64Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The ulong vector.</param>
        public PropVariant(ulong[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromUInt64Vector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The double vector.</param>
        public PropVariant(double[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropVariantNativeMethods.InitPropVariantFromDoubleVector(value, (uint)value.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> vector.</param>
        public PropVariant(DateTime[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var fileTimeArr =
                new System.Runtime.InteropServices.ComTypes.FILETIME[value.Length];

            for (var i = 0; i < value.Length; i++)
            {
                fileTimeArr[i] = DateTimeToFileTime(value[i]);
            }

            PropVariantNativeMethods.InitPropVariantFromFileTimeVector(fileTimeArr, (uint)fileTimeArr.Length, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The bool value.</param>
        public PropVariant(bool value)
        {
            this.valueType = (ushort)VarEnum.VT_BOOL;
            this.int32 = value ? -1 : 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> value.</param>
        public PropVariant(DateTime value)
        {
            this.valueType = (ushort)VarEnum.VT_FILETIME;

            var ft = DateTimeToFileTime(value);
            PropVariantNativeMethods.InitPropVariantFromFileTime(ref ft, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The byte value.</param>
        public PropVariant(byte value)
        {
            this.valueType = (ushort)VarEnum.VT_UI1;
            this.@byte = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The sbyte value.</param>
        public PropVariant(sbyte value)
        {
            this.valueType = (ushort)VarEnum.VT_I1;
            this.@sbyte = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The short value.</param>
        public PropVariant(short value)
        {
            this.valueType = (ushort)VarEnum.VT_I2;
            this.@short = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The ushort value.</param>
        public PropVariant(ushort value)
        {
            this.valueType = (ushort)VarEnum.VT_UI2;
            this.@ushort = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The int value.</param>
        public PropVariant(int value)
        {
            this.valueType = (ushort)VarEnum.VT_I4;
            this.int32 = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The uint value.</param>
        public PropVariant(uint value)
        {
            this.valueType = (ushort)VarEnum.VT_UI4;
            this.uint32 = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        public PropVariant(decimal value)
        {
            this.@decimal = value;

            // It is critical that the value type be set after the decimal value, because they overlap. If valuetype is written first, its
            // value will be lost when _decimal is written.
            this.valueType = (ushort)VarEnum.VT_DECIMAL;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The decimal vector.</param>
        public PropVariant(decimal[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.valueType = (ushort)(VarEnum.VT_DECIMAL | VarEnum.VT_VECTOR);
            this.int32 = value.Length;

            // allocate required memory for array with 128bit elements
            this.blob.Pointer = Marshal.AllocCoTaskMem(value.Length * sizeof(decimal));
            for (var i = 0; i < value.Length; i++)
            {
                var bits = decimal.GetBits(value[i]);
                Marshal.Copy(bits, 0, this.blob.Pointer, bits.Length);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The float value.</param>
        public PropVariant(float value)
        {
            this.valueType = (ushort)VarEnum.VT_R4;

            this.@float = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The float vector.</param>
        public PropVariant(float[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.valueType = (ushort)(VarEnum.VT_R4 | VarEnum.VT_VECTOR);
            this.int32 = value.Length;

            this.blob.Pointer = Marshal.AllocCoTaskMem(value.Length * sizeof(float));

            Marshal.Copy(value, 0, this.blob.Pointer, value.Length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The long value.</param>
        public PropVariant(long value)
        {
            this.@long = value;
            this.valueType = (ushort)VarEnum.VT_I8;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The ulong value.</param>
        public PropVariant(ulong value)
        {
            this.valueType = (ushort)VarEnum.VT_UI8;
            this.@ulong = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropVariant"/> class.
        /// </summary>
        /// <param name="value">The double value.</param>
        public PropVariant(double value)
        {
            this.valueType = (ushort)VarEnum.VT_R8;
            this.@double = value;
        }

        /// <summary>Gets or sets the variant type.</summary>
        public VarEnum VarType
        {
            get => (VarEnum)this.valueType;
            set => this.valueType = (ushort)value;
        }

        /// <summary>Gets a value indicating whether this has an empty or null value.</summary>
        public bool IsNullOrEmpty => this.valueType == (ushort)VarEnum.VT_EMPTY || this.valueType == (ushort)VarEnum.VT_NULL;

        /// <summary>Gets the variant value.</summary>
        public object? Value => (VarEnum)this.valueType switch
        {
            VarEnum.VT_I1 => this.@sbyte,
            VarEnum.VT_UI1 => this.@byte,
            VarEnum.VT_I2 => this.@short,
            VarEnum.VT_UI2 => this.@ushort,
            VarEnum.VT_I4 or VarEnum.VT_INT => this.int32,
            VarEnum.VT_UI4 or VarEnum.VT_UINT => this.uint32,
            VarEnum.VT_I8 => this.@long,
            VarEnum.VT_UI8 => this.@ulong,
            VarEnum.VT_R4 => this.@float,
            VarEnum.VT_R8 => this.@double,
            VarEnum.VT_BOOL => this.int32 == -1,
            VarEnum.VT_ERROR => this.@long,
            VarEnum.VT_CY => this.@decimal,
            VarEnum.VT_DATE => DateTime.FromOADate(this.@double),
            VarEnum.VT_FILETIME => DateTime.FromFileTime(this.@long),
            VarEnum.VT_BSTR => Marshal.PtrToStringBSTR(this.ptr),
            VarEnum.VT_BLOB => this.GetBlobData(),
            VarEnum.VT_LPSTR => Marshal.PtrToStringAnsi(this.ptr),
            VarEnum.VT_LPWSTR => Marshal.PtrToStringUni(this.ptr),
            VarEnum.VT_UNKNOWN => Marshal.GetObjectForIUnknown(this.ptr),
            VarEnum.VT_DISPATCH => Marshal.GetObjectForIUnknown(this.ptr),
            VarEnum.VT_DECIMAL => this.@decimal,
            VarEnum.VT_ARRAY | VarEnum.VT_UNKNOWN => CrackSingleDimSafeArray(this.ptr),
            VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR => this.GetVector<string>(),
            VarEnum.VT_VECTOR | VarEnum.VT_I2 => this.GetVector<short>(),
            VarEnum.VT_VECTOR | VarEnum.VT_UI2 => this.GetVector<ushort>(),
            VarEnum.VT_VECTOR | VarEnum.VT_I4 => this.GetVector<int>(),
            VarEnum.VT_VECTOR | VarEnum.VT_UI4 => this.GetVector<uint>(),
            VarEnum.VT_VECTOR | VarEnum.VT_I8 => this.GetVector<long>(),
            VarEnum.VT_VECTOR | VarEnum.VT_UI8 => this.GetVector<ulong>(),
            VarEnum.VT_VECTOR | VarEnum.VT_R4 => this.GetVector<float>(),
            VarEnum.VT_VECTOR | VarEnum.VT_R8 => this.GetVector<double>(),
            VarEnum.VT_VECTOR | VarEnum.VT_BOOL => this.GetVector<bool>(),
            VarEnum.VT_VECTOR | VarEnum.VT_FILETIME => this.GetVector<DateTime>(),
            VarEnum.VT_VECTOR | VarEnum.VT_DECIMAL => this.GetVector<decimal>(),
            _ => null, // if the value cannot be marshaled
        };

        /// <summary>Attempts to create a PropVariant by finding an appropriate constructor.</summary>
        /// <param name="value">Object from which PropVariant should be created.</param>
        /// <returns>The PropVariant.</returns>
        public static PropVariant FromObject(object value)
        {
            if (value is null)
            {
                return new PropVariant();
            }

            var func = GetDynamicConstructor(value.GetType());
            return func(value);
        }

        /// <summary>Disposes the object, calls the clear function.</summary>
        public void Dispose()
        {
            PropVariantNativeMethods.PropVariantClear(this);

            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public override string ToString() => string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            "{0}: {1}",
            this.Value,
            this.VarType.ToString());

        /// <summary>Set an IUnknown value.</summary>
        /// <param name="value">The new value to set.</param>
        internal void SetIUnknown(object value)
        {
            this.valueType = (ushort)VarEnum.VT_UNKNOWN;
            this.ptr = Marshal.GetIUnknownForObject(value);
        }

        /// <summary>Set a safe array value.</summary>
        /// <param name="array">The new value to set.</param>
        internal void SetSafeArray(Array array)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            const ushort vtUnknown = 13;
            var psa = PropVariantNativeMethods.SafeArrayCreateVector(vtUnknown, 0, (uint)array.Length);

            var pvData = PropVariantNativeMethods.SafeArrayAccessData(psa);
            try
            {
                // to remember to release lock on data
                for (var i = 0; i < array.Length; ++i)
                {
                    var obj = array.GetValue(i);
                    var punk = (obj is not null) ? Marshal.GetIUnknownForObject(obj) : IntPtr.Zero;
                    Marshal.WriteIntPtr(pvData, i * IntPtr.Size, punk);
                }
            }
            finally
            {
                PropVariantNativeMethods.SafeArrayUnaccessData(psa);
            }

            this.valueType = (ushort)VarEnum.VT_ARRAY | (ushort)VarEnum.VT_UNKNOWN;
            this.ptr = psa;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required")]
        private static Dictionary<Type, Action<PropVariant, Array, uint>> GenerateVectorActions() =>
            new()
            {
                {
                    typeof(short),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetInt16Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(ushort),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetUInt16Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(int),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetInt32Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(uint),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetUInt32Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(long),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetInt64Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(ulong),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetUInt64Elem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(DateTime),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetFileTimeElem(pv, i, out var val);

                        var fileTime = GetFileTimeAsLong(ref val);

                        array.SetValue(DateTime.FromFileTime(fileTime), i);
                    }
                },
                {
                    typeof(bool),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetBooleanElem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(double),
                    (pv, array, i) =>
                    {
                        PropVariantNativeMethods.PropVariantGetDoubleElem(pv, i, out var val);
                        array.SetValue(val, i);
                    }
                },
                {
                    typeof(float),
                    (pv, array, i) => // float
                    {
                        var val = new float[1];
                        Marshal.Copy(pv.blob.Pointer, val, (int)i, 1);
                        array.SetValue(val[0], (int)i);
                    }
                },
                {
                    typeof(decimal),
                    (pv, array, i) =>
                    {
                        var val = new int[4];
                        for (var a = 0; a < val.Length; a++)
                        {
                            // index * size + offset quarter
                            val[a] = Marshal.ReadInt32(
                                pv.blob.Pointer,
                                ((int)i * sizeof(decimal)) + (a * sizeof(int)));
                        }

                        array.SetValue(new decimal(val), i);
                    }
                },
                {
                    typeof(string),
                    (pv, array, i) =>
                    {
                        var val = string.Empty;
                        PropVariantNativeMethods.PropVariantGetStringElem(pv, i, ref val);
                        array.SetValue(val, i);
                    }
                },
            };

        // Retrieves a cached constructor expression. If no constructor has been cached, it attempts to find/add it. If it cannot be found an
        // exception is thrown. This method looks for a public constructor with the same parameter type as the object.
        private static Func<object, PropVariant> GetDynamicConstructor(Type type)
        {
            lock (Padlock)
            {
                // initial check, if action is found, return it
                if (!Cache.TryGetValue(type, out var action))
                {
                    // iterates through all constructors
                    var constructor = typeof(PropVariant)
                        .GetConstructor(new Type[] { type });

                    if (constructor is null)
                    {
                        // if the method was not found, throw.
                        throw new ArgumentException(
                            LocalizedMessages.PropVariantTypeNotSupported,
                            nameof(type));
                    }

                    // if the method was found, create an expression to call it.

                    // create parameters to action
                    var arg = Expression.Parameter(typeof(object), "arg");

                    // create an expression to invoke the constructor with an argument cast to the correct type
                    var create = Expression.New(constructor, Expression.Convert(arg, type));

                    // compiles expression into an action delegate
                    action = Expression.Lambda<Func<object, PropVariant>>(create, arg).Compile();
                    Cache.Add(type, action);
                }

                return action;
            }
        }

        private static long GetFileTimeAsLong(ref System.Runtime.InteropServices.ComTypes.FILETIME val) => (((long)val.dwHighDateTime) << 32) + val.dwLowDateTime;

        private static System.Runtime.InteropServices.ComTypes.FILETIME DateTimeToFileTime(DateTime value)
        {
            var hFT = value.ToFileTime();
            return new System.Runtime.InteropServices.ComTypes.FILETIME
            {
                dwLowDateTime = (int)(hFT & 0xFFFFFFFF),
                dwHighDateTime = (int)(hFT >> 32),
            };
        }

        private static Array CrackSingleDimSafeArray(IntPtr psa)
        {
            var cDims = PropVariantNativeMethods.SafeArrayGetDim(psa);
            if (cDims != 1)
            {
                throw new ArgumentException(LocalizedMessages.PropVariantMultiDimArray, nameof(psa));
            }

            var lBound = PropVariantNativeMethods.SafeArrayGetLBound(psa, 1U);
            var uBound = PropVariantNativeMethods.SafeArrayGetUBound(psa, 1U);

            var n = uBound - lBound + 1; // uBound is inclusive

            var array = new object[n];
            for (var i = lBound; i <= uBound; ++i)
            {
                array[i] = PropVariantNativeMethods.SafeArrayGetElement(psa, ref i);
            }

            return array;
        }

        private object GetBlobData()
        {
            var blobData = new byte[this.int32];

            var pBlobData = this.blob.Pointer;
            Marshal.Copy(pBlobData, blobData, 0, this.int32);

            return blobData;
        }

        private Array GetVector<T>()
        {
            var count = PropVariantNativeMethods.PropVariantGetElementCount(this);
            if (count <= 0)
            {
                return Array.Empty<object>();
            }

            lock (Padlock)
            {
                if (vectorActions is null)
                {
#pragma warning disable S2696 // Instance members should not write to "static" fields
                    vectorActions = GenerateVectorActions();
#pragma warning restore S2696 // Instance members should not write to "static" fields
                }
            }

            if (!vectorActions.TryGetValue(typeof(T), out var action))
            {
                throw new InvalidCastException(LocalizedMessages.PropVariantUnsupportedType);
            }

            Array array = new T[count];
            for (uint i = 0; i < count; i++)
            {
                action(this, array, i);
            }

            return array;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Blob
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "This is a false positive")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144", Justification = "This is required for padding")]
            public int Number;
            public IntPtr Pointer;
        }
#pragma warning restore CS0618, RCS1130, S3265
    }
}
