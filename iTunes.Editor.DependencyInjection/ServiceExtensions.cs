// -----------------------------------------------------------------------
// <copyright file="ServiceExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Humanizer;
    using ITunes.Editor;
    using Neleus.DependencyInjection.Extensions;

    /// <summary>
    /// The extentions.
    /// </summary>
    public static class ServiceExtensions
    {
        private static readonly System.Collections.IList ServiceByNameBuilders = new System.Collections.Generic.List<object>();

        /// <summary>
        /// Adds API Seeds.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddApiSeeds(this IServiceCollection serviceCollection, Configuration.IConfiguration configuration) => serviceCollection
            .Configure<ITunes.Editor.ApiSeeds.ApiSeedsOptions>(configuration?.GetSection(nameof(ITunes.Editor.ApiSeeds)))
            .AddTransient<ILyricsProvider, ITunes.Editor.ApiSeeds.ApiSeedsLyricsProvider>(GetLyricsProviderName(nameof(ITunes.Editor.ApiSeeds)));

        /// <summary>
        /// Adds Chart Lyrics.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddChartLyrics(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.ChartLyrics.ChartLyricsLyricsProvider>(GetLyricsProviderName(nameof(ITunes.Editor.ChartLyrics)));

        /// <summary>
        /// Adds Genius.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddGenius(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.Lyrics.Genius.GeniusLyricsProvider>(GetLyricsProviderName(nameof(ITunes.Editor.Lyrics.Genius)));

        /// <summary>
        /// Adds AZ.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddAZ(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.Lyrics.AZLyrics.AZLyricsProvider>(GetLyricsProviderName(nameof(ITunes.Editor.Lyrics.AZLyrics)));

        /// <summary>
        /// Adds OVH.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddOvh(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.Lyrics.Ovh.OvhLyricsProvider>(GetLyricsProviderName(nameof(ITunes.Editor.Lyrics.Ovh)));

        /// <summary>
        /// Adds happi.dev.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddHappiDev(this IServiceCollection serviceCollection, Configuration.IConfiguration configuration)
        {
            serviceCollection
                .AddHttpClient<ITunes.Editor.HappiDev.HappiDevLyricsProvider>();
            return serviceCollection
                .Configure<ITunes.Editor.HappiDev.HappiDevOptions>(configuration?.GetSection(nameof(ITunes.Editor.HappiDev)))
                .AddTransient<ILyricsProvider, ITunes.Editor.HappiDev.HappiDevLyricsProvider>(GetLyricsProviderName(nameof(ITunes.Editor.HappiDev)));
        }

        /// <summary>
        /// Adds Purgo Malum.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddPurgoMalum(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<IExplicitLyricsProvider, ITunes.Editor.PurgoMalum.PurgoMalumExplicitLyricsProvider>(GetProviderName(nameof(ITunes.Editor.PurgoMalum).Underscore()));

        /// <summary>
        /// Adds Purgo Malum.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddDefault(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<IExplicitLyricsProvider, ListExplicitLyricsProvider>("list");

        /// <summary>
        /// Adds the folder provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddFolder(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ISongsProvider, FolderSongsProvider>("folder");

        /// <summary>
        /// Adds the iPod provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddIPod(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ISongsProvider, ITunes.Editor.IPod.IPodSongsProvider>(GetProviderName(nameof(ITunes.Editor.IPod)));

        /// <summary>
        /// Adds the iTunes provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddITunes(this IServiceCollection serviceCollection)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                serviceCollection.AddTransient<ISongsProvider, ITunes.Editor.ITunesLib.ITunesSongsProvider>(nameof(ITunes).ToLower(System.Globalization.CultureInfo.CurrentCulture));
            }

            return serviceCollection;
        }

        /// <summary>
        /// Adds the shell provider.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <returns>The return service descriptors.</returns>
        public static IServiceCollection AddShell(this IServiceCollection serviceDescriptors)
        {
            if (MS.WindowsAPICodePack.Internal.CoreHelpers.RunningOnVista)
            {
                serviceDescriptors.AddTransient<ISongsProvider, ITunes.Editor.Windows.ShellSongsProvider>("shell");
            }

            return serviceDescriptors;
        }

        /// <summary>
        /// Adds the PList provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddPList(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ISongsProvider, ITunes.Editor.PList.PListSongsProvider>(nameof(ITunes.Editor.PList).ToLower(System.Globalization.CultureInfo.CurrentCulture));

        /// <summary>
        /// Adds the MediaInfo provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddTagLib(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ITagProvider, TagLibTagProvider>(nameof(TagLib).ToLower(System.Globalization.CultureInfo.CurrentCulture));

        /// <summary>
        /// Adds the MediaInfo provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddMediaInfo(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ITagProvider, ITunes.Editor.MediaInfo.MediaInfoTagProvider>(nameof(ITunes.Editor.MediaInfo).ToLower(System.Globalization.CultureInfo.CurrentCulture));

        /// <summary>
        /// Adds the ApraAmcos provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddApraAmcos(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<IComposerProvider, ITunes.Editor.Composers.ApraAmcos.ApraAmcosComposerProvider>("apra_amcos");

        /// <summary>
        /// Get service of type <typeparamref name="T"/> from the <see cref="System.IServiceProvider"/> with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The <see cref="System.IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="key">The key.</param>
        /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
        public static T GetService<T>(this System.IServiceProvider provider, string key) => provider.GetByName<T>(key);

        /// <summary>
        /// Get service of type <typeparamref name="T"/> from the <see cref="System.IServiceProvider"/> with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="provider">The <see cref="System.IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="key">The key.</param>
        /// <returns>A service object of type <typeparamref name="T"/>.</returns>
        /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/>.</exception>
        public static T GetRequiredService<T>(this System.IServiceProvider provider, string key) => provider.GetByName<T>(key) ?? throw new System.InvalidOperationException();

        /// <summary>
        /// Gets the service object of the specified type from the <see cref="System.IServiceProvider"/> with the specified key.
        /// </summary>
        /// <param name="provider">The <see cref="System.IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="serviceType">The service type.</param>
        /// <param name="key">The key.</param>
        /// <returns>A service object of type <paramref name="serviceType"/> --or-- <see langword="null"/> if there is no service object of type <paramref name="serviceType"/>.</returns>
        public static object GetRequiredService(this System.IServiceProvider provider, System.Type serviceType, string key)
        {
            var type = typeof(ServiceExtensions);
            var method = type.GetMethod(nameof(GetRequiredService), new[] { typeof(System.IServiceProvider), typeof(string) });
            if (method is null)
            {
                throw new System.ArgumentException("Could not get method", nameof(provider));
            }

            var genericMethod = method.MakeGenericMethod(serviceType);
            if (genericMethod.Invoke(null, new object[] { provider, key }) is object obj)
            {
                return obj;
            }

            throw new System.Collections.Generic.KeyNotFoundException();
        }

        private static string GetLyricsProviderName(string name) => GetProviderName(name, nameof(ITunes.Editor.Lyrics));

        private static string GetProviderName(string name, string suffix = "")
        {
            var providerName = name;
            if (!string.IsNullOrEmpty(suffix))
            {
                providerName = providerName
#if NETSTANDARD2_0
                    .Replace(suffix, string.Empty);
#else
                    .Replace(suffix, string.Empty, System.StringComparison.OrdinalIgnoreCase);
#endif
            }

            return providerName.ToLower(System.Globalization.CultureInfo.CurrentCulture);
        }

        private static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection serviceCollection, string name)
            where TService : class
            where TImplementation : class, TService
        {
            ServicesByNameBuilder<TService> GetOrAddByName()
            {
                foreach (var builder in ServiceByNameBuilders)
                {
                    if (builder is ServicesByNameBuilder<TService> typedBuilder)
                    {
                        return typedBuilder;
                    }
                }

                var serviceByNameBuilder = serviceCollection.AddByName<TService>();
                ServiceByNameBuilders.Add(serviceByNameBuilder);
                return serviceByNameBuilder;
            }

            serviceCollection
                .AddTransient<TService, TImplementation>()
                .AddTransient<TImplementation>();

            var builder = GetOrAddByName()
                .Add<TImplementation>(name);

            var service = serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(IServiceByNameFactory<TService>));
            if (service is not null)
            {
                serviceCollection.Remove(service);
            }

            builder.Build();
            return serviceCollection;
        }
    }
}
