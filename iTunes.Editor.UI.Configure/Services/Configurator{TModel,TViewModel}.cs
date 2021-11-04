// <copyright file="Configurator{TModel,TViewModel}.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Services;

/// <summary>
/// The configurator.
/// </summary>
/// <typeparam name="TModel">The type to configure.</typeparam>
/// <typeparam name="TViewModel">The view model.</typeparam>
public abstract class Configurator<TModel, TViewModel> : IConfigurator<TModel>
    where TViewModel : Models.IConfigure
{
    private readonly System.Func<TModel, TViewModel> viewModelCreator;

    /// <summary>
    /// Initializes a new instance of the <see cref="Configurator{TModel, TViewModel}"/> class.
    /// </summary>
    /// <param name="viewModelCreator">The view model creator.</param>
    protected Configurator(System.Func<TModel, TViewModel> viewModelCreator) => this.viewModelCreator = viewModelCreator;

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; } = "Configure";

    /// <inheritdoc/>
    public bool Configure(TModel source) => throw new System.NotSupportedException();

    /// <inheritdoc/>
    public System.Threading.Tasks.ValueTask<bool> ConfigureAsync(TModel source) => this.ConfigureViewModelAsync(this.viewModelCreator(source));

    /// <inheritdoc cref="IConfigurator{T}.ConfigureAsync(T)"/>
    protected abstract System.Threading.Tasks.ValueTask<bool> ConfigureViewModelAsync(TViewModel source);
}
