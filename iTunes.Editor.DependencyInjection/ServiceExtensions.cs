// <copyright file="ServiceExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
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
            .AddTransient<ILyricsProvider, ITunes.Editor.ApiSeeds.ApiSeedsLyricsProvider>("apiseeds")
            .Configure<ITunes.Editor.ApiSeeds.ApiSeeds>(configuration?.GetSection("ApiSeeds"));

        /// <summary>
        /// Adds Chart Lyrics.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddChartLyrics(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.ChartLyrics.ChartLyricsLyricsProvider>("chart");

        /// <summary>
        /// Adds Wikia.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddWikia(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.Lyrics.Wikia.WikiaLyricsProvider>("wikia");

        /// <summary>
        /// Adds Genius.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddGenius(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.Lyrics.Genius.GeniusLyricsProvider>("genius");

        /// <summary>
        /// Adds AZ.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddAZ(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.Lyrics.AZLyrics.AZLyricsProvider>("az");

        /// <summary>
        /// Adds OVH.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddOvh(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ILyricsProvider, ITunes.Editor.Lyrics.Ovh.OvhLyricsProvider>("ovh");

        /// <summary>
        /// Adds Purgo Malum.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddPurgoMalum(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<IExplicitLyricsProvider, ITunes.Editor.PurgoMalum.PurgoMalumExplicitLyricsProvider>("purgo_malum");

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
            .AddTransient<ISongsProvider, ITunes.Editor.IPod.IPodSongsProvider>("ipod");

        /// <summary>
        /// Adds the iTunes provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddITunes(this IServiceCollection serviceCollection)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                serviceCollection.AddTransient<ISongsProvider, ITunes.Editor.ITunesLib.ITunesSongsProvider>("itunes");
            }

            return serviceCollection;
        }

        /// <summary>
        /// Adds the PList provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddPList(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ISongsProvider, ITunes.Editor.PList.PListSongsProvider>("plist");

        /// <summary>
        /// Adds the MediaInfo provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddTagLib(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ITagProvider, TagLibTagProvider>("taglib");

        /// <summary>
        /// Adds the MediaInfo provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddMediaInfo(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<ITagProvider, ITunes.Editor.MediaInfo.MediaInfoTagProvider>("mediainfo");

        /// <summary>
        /// Adds the ApraAmcos provider.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The return service collection.</returns>
        public static IServiceCollection AddApraAmcos(this IServiceCollection serviceCollection) => serviceCollection
            .AddTransient<IComposerProvider, ApraAmcosComposerProvider>("apra_amcos");

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
        public static object GetRequiredService(this System.IServiceProvider provider, System.Type serviceType, string key) => typeof(ServiceExtensions)
            .GetMethod(nameof(GetRequiredService), new[] { typeof(System.IServiceProvider), typeof(string) })
            .MakeGenericMethod(serviceType)
            .Invoke(null, new object[] { provider, key });

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
            if (service != null)
            {
                serviceCollection.Remove(service);
            }

            builder.Build();
            return serviceCollection;
        }
    }
}
