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
    private readonly Func<TModel, TViewModel> viewModelCreator;

    /// <summary>
    /// Initializes a new instance of the <see cref="Configurator{TModel, TViewModel}"/> class.
    /// </summary>
    /// <param name="viewModelCreator">The view model creator.</param>
    protected Configurator(Func<TModel, TViewModel> viewModelCreator) => this.viewModelCreator = viewModelCreator;

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; } = "Configure";

    /// <inheritdoc/>
    public bool Configure(TModel source) => throw new NotSupportedException();

    /// <inheritdoc/>
    public ValueTask<bool> ConfigureAsync(TModel source) => this.ConfigureViewModelAsync(this.viewModelCreator(source));

    /// <inheritdoc cref="IConfigurator{T}.ConfigureAsync(T)"/>
    protected abstract ValueTask<bool> ConfigureViewModelAsync(TViewModel source);
}
