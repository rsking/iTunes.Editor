// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Caliburn.Micro;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : CaliburnApplication
    {
        /// <summary>
        /// The container.
        /// </summary>
        private readonly WinRTContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class as the singleton application object.
        /// This is the first line of authored code executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.container = new WinRTContainer();
            this.Initialize();
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void Configure()
        {
            this.container.RegisterWinRTServices();
            this.container.PerRequest<ViewModels.HomeViewModel>();
        }

        /// <inheritdoc/>
        protected override void PrepareViewFirst(Frame rootFrame)
        {
            this.container.RegisterNavigationService(rootFrame);
        }

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                return;
            }

            this.DisplayRootView<Views.HomeView>();
        }

        /// <inheritdoc/>
        protected override object GetInstance(Type service, string key)
        {
            return this.container.GetInstance(service, key);
        }

        /// <inheritdoc/>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.container.GetAllInstances(service);
        }

        /// <inheritdoc/>
        protected override void BuildUp(object instance)
        {
            this.container.BuildUp(instance);
        }
    }
}
