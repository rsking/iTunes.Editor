// -----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using Ninject;

    /// <summary>
    /// The boot strapper.
    /// </summary>
    public class Bootstrapper : Caliburn.Micro.BootstrapperBase
    {
        private readonly IKernel container;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        public Bootstrapper()
        {
            this.container = new StandardKernel();
            this.Initialize();
        }

        /// <inheritdoc/>
        protected override void Configure()
        {
            this.container.Bind<Caliburn.Micro.IWindowManager>().To<MetroWindowManager>().InSingletonScope();
        }

        /// <inheritdoc/>
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e) => this.DisplayRootViewFor<ViewModels.ShellViewModel>();

        /// <inheritdoc/>
        protected override object GetInstance(Type service, string key) => string.IsNullOrEmpty(key)
            ? this.container.Get(service)
            : this.container.Get(service, key);

        /// <inheritdoc/>
        protected override IEnumerable<object> GetAllInstances(Type service) => this.container.GetAll(service);

        /// <inheritdoc/>
        protected override void BuildUp(object instance) => this.container.Inject(instance);
    }
}
