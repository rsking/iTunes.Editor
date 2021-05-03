// <copyright file="ShellNativeMethods.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell
{
    using System;
    using System.Runtime.InteropServices;

#pragma warning disable MA0062, S4070, SA1600, SA1602
    /// <summary>
    /// The shell native methods.
    /// </summary>
    internal static class ShellNativeMethods
    {
        /// <summary>
        /// Indicate flags that modify the property store object retrieved by methods that create a property store, such as
        /// IShellItem2::GetPropertyStore or IPropertyStoreFactory::GetPropertyStore.
        /// </summary>
        [Flags]
        internal enum GetPropertyStoreOptions
        {
            None = 0,

            /// <summary>
            /// Meaning to a calling process: Return a read-only property store that contains all properties. Slow items (offline files) are
            /// not opened. Combination with other flags: Can be overridden by other flags.
            /// </summary>
            Default = None,

            /// <summary>
            /// <para>
            /// Meaning to a calling process: Include only properties directly from the property handler, which opens the file on the disk,
            /// network, or device. Meaning to a file
            /// folder: Only include properties directly from the handler.
            /// </para>
            /// <para>
            /// Meaning to other folders: When delegating to a file folder, pass this flag on to the file folder; do not do any multiplexing
            /// (MUX). When not delegating to a file folder, ignore this flag instead of returning a failure code.
            /// </para>
            /// <para>
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_FASTPROPERTIESONLY, or GPS_BESTEFFORT.
            /// </para>
            /// </summary>
            HandlePropertiesOnly = 1,

            /// <summary>
            /// <para>
            /// Meaning to a calling process: Can write properties to the item.
            /// Note: The store may contain fewer properties than a read-only store.
            /// </para>
            /// <para>
            /// Meaning to a file folder: ReadWrite.
            /// </para>
            /// <para>
            /// Meaning to other folders: ReadWrite. Note: When using default MUX, return a single unmultiplexed store because the default
            /// MUX does not support ReadWrite.
            /// </para>
            /// <para>
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_FASTPROPERTIESONLY, GPS_BESTEFFORT, or
            /// GPS_DELAYCREATION. Implies GPS_HANDLERPROPERTIESONLY.
            /// </para>
            /// </summary>
            ReadWrite = 1 << 1,

            /// <summary>
            /// <para>
            /// Meaning to a calling process: Provides a writable store, with no initial properties, that exists for the lifetime of the
            /// Shell item instance; basically, a property bag attached to the item instance.
            /// </para>
            /// <para>
            /// Meaning to a file folder: Not applicable. Handled by the Shell item.
            /// </para>
            /// <para>
            /// Meaning to other folders: Not applicable. Handled by the Shell item.
            /// </para>
            /// <para>
            /// Combination with other flags: Cannot be combined with any other flag. Implies GPS_READWRITE.
            /// </para>
            /// </summary>
            Temporary = 1 << 2,

            /// <summary>
            /// <para>
            /// Meaning to a calling process: Provides a store that does not involve reading from the disk or network. Note: Some values may
            /// be different, or missing, compared to a store without this flag.
            /// </para>
            /// <para>
            /// Meaning to a file folder: Include the "innate" and "fallback" stores only. Do not load the handler.
            /// </para>
            /// <para>
            /// Meaning to other folders: Include only properties that are available in memory or can be computed very quickly (no properties
            /// from disk, network, or peripheral IO devices). This is normally only data sources from the IDLIST. When delegating to other
            /// folders, pass this flag on to them.
            /// </para>
            /// <para>
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_READWRITE, GPS_HANDLERPROPERTIESONLY, or GPS_DELAYCREATION.
            /// </para>
            /// </summary>
            FastPropertiesOnly = 1 << 3,

            /// <summary>
            /// <para>
            /// Meaning to a calling process: Open a slow item (offline file) if necessary. Meaning to a file folder: Retrieve a file from
            /// offline storage, if necessary.
            /// Note: Without this flag, the handler is not created for offline files.
            /// </para>
            /// <para>
            /// Meaning to other folders: Do not return any properties that are very slow.
            /// </para>
            /// <para>
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY or GPS_FASTPROPERTIESONLY.
            /// </para>
            /// </summary>
            OpensLowItem = 1 << 4,

            /// <summary>
            /// <para>
            /// Meaning to a calling process: Delay memory-intensive operations, such as file access, until a property is requested that
            /// requires such access.
            /// </para>
            /// <para>
            /// Meaning to a file folder: Do not create the handler until needed; for example, either GetCount/GetAt or GetValue, where the
            /// innate store does not satisfy the request.
            /// Note: GetValue might fail due to file access problems.
            /// </para>
            /// <para>
            /// Meaning to other folders: If the folder has memory-intensive properties, such as delegating to a file folder or network
            /// access, it can optimize performance by supporting IDelayedPropertyStoreFactory and splitting up its properties into a fast
            /// and a slow store. It can then use delayed MUX to recombine them.
            /// </para>
            /// <para>
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY or GPS_READWRITE.
            /// </para>
            /// </summary>
            DelayCreation = 1 << 5,

            /// <summary>
            /// <para>
            /// Meaning to a calling process: Succeed at getting the store, even if some properties are not returned. Note: Some values may
            /// be different, or missing, compared to a store without this flag.
            /// </para>
            /// <para>
            /// Meaning to a file folder: Succeed and return a store, even if the handler or innate store has an error during creation. Only
            /// fail if substores fail.
            /// </para>
            /// <para>
            /// Meaning to other folders: Succeed on getting the store, even if some properties are not returned.
            /// </para>
            /// <para>
            /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_READWRITE, or GPS_HANDLERPROPERTIESONLY.
            /// </para>
            /// </summary>
            BestEffort = 1 << 6,

            /// <summary>Mask for valid GETPROPERTYSTOREFLAGS values.</summary>
            MaskValid = 0xff,
        }

        [Flags]
        internal enum ShellFileGetAttributesOptions
        {
            /// <summary>The specified items can be copied.</summary>
            CanCopy = 1,

            /// <summary>The specified items can be moved.</summary>
            CanMove = 1 << 1,

            /// <summary>
            /// Shortcuts can be created for the specified items. This flag has the same value as DROPEFFECT. The normal use of this flag is
            /// to add a Create Shortcut item to the shortcut menu that is displayed during drag-and-drop operations. However, SFGAO_CANLINK
            /// also adds a Create Shortcut item to the Microsoft Windows Explorer's File menu and to normal shortcut menus. If this item is
            /// selected, your application's IContextMenu::InvokeCommand is invoked with the lpVerb member of the CMINVOKECOMMANDINFO
            /// structure set to "link." Your application is responsible for creating the link.
            /// </summary>
            CanLink = 1 << 2,

            /// <summary>The specified items can be bound to an IStorage interface through IShellFolder::BindToObject.</summary>
            Storage = 1 << 3,

            /// <summary>The specified items can be renamed.</summary>
            CanRename = 1 << 4,

            /// <summary>The specified items can be deleted.</summary>
            CanDelete = 1 << 5,

            /// <summary>The specified items have property sheets.</summary>
            HasPropertySheet = 1 << 6,

            /// <summary>The specified items are drop targets.</summary>
            DropTarget = 1 << 8,

            /// <summary>This flag is a mask for the capability flags.</summary>
            CapabilityMask = CanCopy | CanMove | CanLink | CanRename | CanDelete | HasPropertySheet | DropTarget,

            /// <summary>Windows 7 and later. The specified items are system items.</summary>
            System = 1 << 12,

            /// <summary>The specified items are encrypted.</summary>
            Encrypted = 1 << 13,

            /// <summary>
            /// Indicates that accessing the object = through IStream or other storage interfaces, is a slow operation. Applications should
            /// avoid accessing items flagged with SFGAO_ISSLOW.
            /// </summary>
            IsSlow = 1 << 14,

            /// <summary>The specified items are ghosted icons.</summary>
            Ghosted = 1 << 15,

            /// <summary>The specified items are shortcuts.</summary>
            Link = 1 << 16,

            /// <summary>The specified folder objects are shared.</summary>
            Share = 1 << 17,

            /// <summary>
            /// The specified items are read-only. In the case of folders, this means that new items cannot be created in those folders.
            /// </summary>
            ReadOnly = 1 << 18,

            /// <summary>
            /// The item is hidden and should not be displayed unless the Show hidden files and folders option is enabled in Folder Settings.
            /// </summary>
            Hidden = 1 << 19,

            /// <summary>This flag is a mask for the display attributes.</summary>
            DisplayAttributeMask = IsSlow | Ghosted | Link | Share | ReadOnly | Hidden,

            /// <summary>The specified folders contain one or more file system folders.</summary>
            FileSystemAncestor = 1 << 28,

            /// <summary>The specified items are folders.</summary>
            Folder = 1 << 29,

            /// <summary>
            /// The specified folders or file objects are part of the file system that is, they are files, directories, or root directories).
            /// </summary>
            FileSystem = 1 << 30,

            /// <summary>The specified folders have subfolders = and are, therefore, expandable in the left pane of Windows Explorer).</summary>
            HasSubFolder = unchecked((int)0x80000000),

            /// <summary>This flag is a mask for the contents attributes.</summary>
            ContentsMask = HasSubFolder,

            /// <summary>
            /// When specified as input, SFGAO_VALIDATE instructs the folder to validate that the items pointed to by the contents of apidl
            /// exist. If one or more of those items do not exist, IShellFolder::GetAttributesOf returns a failure code. When used with the
            /// file system folder, SFGAO_VALIDATE instructs the folder to discard cached properties retrieved by clients of
            /// IShellFolder2::GetDetailsEx that may have accumulated for the specified items.
            /// </summary>
            Validate = 1 << 24,

            /// <summary>The specified items are on removable media or are themselves removable devices.</summary>
            Removable = 1 << 25,

            /// <summary>The specified items are compressed.</summary>
            Compressed = 1 << 26,

            /// <summary>The specified items can be browsed in place.</summary>
            Browsable = 1 << 27,

            /// <summary>The items are nonenumerated items.</summary>
            Nonenumerated = 1 << 20,

            /// <summary>The objects contain new content.</summary>
            NewContent = 1 << 21,

            /// <summary>It is possible to create monikers for the specified file objects or folders.</summary>
            CanMoniker = 1 << 22,

            /// <summary>Not supported.</summary>
            HasStorage = CanMoniker,

            /// <summary>
            /// Indicates that the item has a stream associated with it that can be accessed by a call to IShellFolder::BindToObject with
            /// IID_IStream in the riid parameter.
            /// </summary>
            Stream = CanMoniker,

            /// <summary>
            /// Children of this item are accessible through IStream or IStorage. Those children are flagged with SFGAO_STORAGE or SFGAO_STREAM.
            /// </summary>
            StorageAncestor = 1 << 23,

            /// <summary>This flag is a mask for the storage capability attributes.</summary>
            StorageCapabilityMask = Storage | Link | ReadOnly | CanMoniker | StorageAncestor | FileSystemAncestor | Folder | FileSystem,

            /// <summary>
            /// Mask used by PKEY_SFGAOFlags to remove certain values that are considered to cause slow calculations or lack context. Equal
            /// to SFGAO_VALIDATE | SFGAO_ISSLOW | SFGAO_HASSUBFOLDER.
            /// </summary>
            PkeyMask = ContentsMask | IsSlow | ReadOnly | Validate,
        }

        [Flags]
        internal enum ShellFolderEnumerationOptions : ushort
        {
            CheckingForChildren = 1 << 4,
            Folders = 1 << 5,
            NonFolders = 1 << 6,
            IncludeHidden = 1 << 7,
            InitializeOnFirstNext = 1 << 8,
            NetPrinterSearch = 1 << 9,
            Shareable = 1 << 10,
            Storage = 1 << 11,
            NavigationEnum = 1 << 12,
            FastItems = 1 << 13,
            FlatList = 1 << 14,
            EnableAsync = 1 << 15,
        }

        internal enum ShellItemDesignNameOptions
        {
            ParentRelativeParsing = unchecked((int)0x80018001),
            DesktopAbsoluteParsing = unchecked((int)0x80028000),
            ParentRelativeEditing = unchecked((int)0x80031001),
            DesktopAbsoluteEditing = unchecked((int)0x8004c000),
            FileSystemPath = unchecked((int)0x80058000),
            Url = unchecked((int)0x80068000),
            ParentRelativeForAddressBar = unchecked((int)0x8007c001),
            ParentRelative = unchecked((int)0x80080001),
            Normal = 0x00000000,
        }

        [DllImport("shell32.dll", CharSet = CharSet.None)]
        public static extern void ILFree(IntPtr pidl);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint ILGetSize(IntPtr pidl);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string path,
            IntPtr pbc, // This parameter is not used - binding context.
            ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHGetIDListFromObject(
            IntPtr iUnknown,
            out IntPtr ppidl);
    }
#pragma warning disable MA0062, S4070, SA1600, SA1602
}
