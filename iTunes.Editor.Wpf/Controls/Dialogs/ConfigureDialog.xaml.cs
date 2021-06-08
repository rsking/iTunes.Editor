// <copyright file="ConfigureDialog.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Controls.Dialogs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;

    /// <summary>
    /// Interaction logic for <c>ConfigureDialog.xaml</c>.
    /// </summary>
    public partial class ConfigureDialog : BaseMetroDialog
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

        private CancellationTokenRegistration cancellationTokenRegistration;

        private RoutedEventHandler? negativeHandler;
        private KeyEventHandler? negativeKeyHandler;
        private RoutedEventHandler? affirmativeHandler;
        private KeyEventHandler? affirmativeKeyHandler;
        private KeyEventHandler? escapeKeyHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureDialog"/> class.
        /// </summary>
        internal ConfigureDialog()
            : this(parentWindow: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureDialog"/> class.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        internal ConfigureDialog(MetroWindow? parentWindow)
            : this(parentWindow, settings: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureDialog"/> class.
        /// </summary>
        /// <param name="parentWindow">The parent window.</param>
        /// <param name="settings">The settings.</param>
        internal ConfigureDialog(MetroWindow? parentWindow, MetroDialogSettings? settings)
            : base(parentWindow, settings) => this.InitializeComponent();

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

        /// <summary>
        /// Waits for the button press.
        /// </summary>
        /// <returns>The result.</returns>
        internal Task<bool> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() => this.Focus()));

            var tcs = new TaskCompletionSource<bool>();

            void CleanUpHandlers()
            {
                this.KeyDown -= this.escapeKeyHandler;

                this.PART_NegativeButton.Click -= this.negativeHandler;
                this.PART_AffirmativeButton.Click -= this.affirmativeHandler;

                this.PART_NegativeButton.KeyDown -= this.negativeKeyHandler;
                this.PART_AffirmativeButton.KeyDown -= this.affirmativeKeyHandler;

                this.cancellationTokenRegistration.Dispose();
            }

            this.cancellationTokenRegistration = this.DialogSettings
                .CancellationToken
                .Register(() => this.BeginInvoke(() =>
                {
                    CleanUpHandlers();
                    tcs.TrySetResult(false);
                }));

            this.escapeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(false);
                }
            };

            this.negativeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(false);
                }
            };

            this.affirmativeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(true);
                }
            };

            this.negativeHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(false);

                e.Handled = true;
            };

            this.affirmativeHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(true);

                e.Handled = true;
            };

            this.PART_NegativeButton.KeyDown += this.negativeKeyHandler;
            this.PART_AffirmativeButton.KeyDown += this.affirmativeKeyHandler;

            this.KeyDown += this.escapeKeyHandler;

            this.PART_NegativeButton.Click += this.negativeHandler;
            this.PART_AffirmativeButton.Click += this.affirmativeHandler;

            return tcs.Task;
        }

        /// <inheritdoc/>
        protected override void OnLoaded()
        {
            this.AffirmativeButtonText = this.DialogSettings.AffirmativeButtonText;
            this.NegativeButtonText = this.DialogSettings.NegativeButtonText;

            switch (this.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_NegativeButton.SetResourceReference(StyleProperty, "MahApps.Styles.Button.Dialogs.AccentHighlight");
                    break;
            }
        }
    }
}
