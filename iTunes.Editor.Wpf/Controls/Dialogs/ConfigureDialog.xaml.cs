// <copyright file="ConfigureDialog.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Controls.Dialogs;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// Interaction logic for <c>ConfigureDialog.xaml</c>.
/// </summary>
public partial class ConfigureDialog
{
    /// <summary>Identifies the <see cref="AffirmativeButtonText"/> dependency property.</summary>
    public static readonly DependencyProperty AffirmativeButtonTextProperty
        = DependencyProperty.Register(
            nameof(AffirmativeButtonText),
            typeof(string),
            typeof(ConfigureDialog),
            new PropertyMetadata("OK"));

    /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
    public static readonly DependencyProperty NegativeButtonTextProperty
        = DependencyProperty.Register(
            nameof(NegativeButtonText),
            typeof(string),
            typeof(ConfigureDialog),
            new PropertyMetadata("Cancel"));

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureDialog"/> class.
    /// </summary>
    internal ConfigureDialog()
    {
        this.InitializeComponent();
        if (LogicalTreeHelper.FindLogicalNode(this, "PART_AffirmativeButton") is System.Windows.Controls.Primitives.ButtonBase button)
        {
            button.Click += (s, e) => this.DialogResult = true;
        }
    }

    /// <summary>
    /// Gets or sets the affirmative button text.
    /// </summary>
    public string AffirmativeButtonText
    {
        get => (string)this.GetValue(AffirmativeButtonTextProperty);
        set => this.SetValue(AffirmativeButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the negative button text.
    /// </summary>
    public string NegativeButtonText
    {
        get => (string)this.GetValue(NegativeButtonTextProperty);
        set => this.SetValue(NegativeButtonTextProperty, value);
    }
}
