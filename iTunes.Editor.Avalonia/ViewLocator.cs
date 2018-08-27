// -----------------------------------------------------------------------
// <copyright file="ViewLocator.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using Avalonia.Controls;
    using Avalonia.Controls.Templates;

    /// <summary>
    /// The view locator.
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// Gets a value indicating whether this instance supports recycling.
        /// </summary>
        public bool SupportsRecycling => false;

        /// <summary>
        /// Builds the control.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The created control.</returns>
        public IControl Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                var control = (Control)Activator.CreateInstance(type);
                Avalonia.Markup.Xaml.AvaloniaXamlLoaderPortableXaml.Load(control);
                return control;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        /// <summary>
        /// Mathes the instance.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Whether the data is matched.</returns>
        public bool Match(object data)
        {
            if (data == null)
            {
                return false;
            }

            var name = data.GetType().FullName.Replace("ViewModel", "View");
            return Type.GetType(name) != null;
        }
    }
}