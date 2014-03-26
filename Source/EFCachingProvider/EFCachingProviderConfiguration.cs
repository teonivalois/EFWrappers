// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Configuration;
using EFCachingProvider.Caching;
using EFProviderWrapperToolkit;

namespace EFCachingProvider
{
    /// <summary>
    /// Default configuration settings for EFCachingProvider.
    /// </summary>
    public static class EFCachingProviderConfiguration
    {
        /// <summary>
        /// Initializes static members of the EFCachingProviderConfiguration class.
        /// </summary>
        static EFCachingProviderConfiguration()
        {
            DefaultCachingPolicy = CachingPolicy.NoCaching;
            DefaultWrappedFactory = ConfigurationManager.AppSettings["EFCachingProvider.wrappedProvider"];
            DefaultWrappedServices = ConfigurationManager.AppSettings["EFCachingProvider.wrappedServices"];
        }

        /// <summary>
        /// Gets or sets the default wrapped factory.
        /// </summary>
        /// <value>The default wrapped factory.</value>
        public static string DefaultWrappedFactory { get; set; }

        /// <summary>
        /// Gets or sets the default wrapped services.
        /// </summary>
        /// <value>The default wrapped services.</value>
        public static string DefaultWrappedServices { get; set; }

        /// <summary>
        /// Gets or sets default caching <see cref="ICache"/> implementation which should be used for new connections.
        /// </summary>
        public static ICache DefaultCache { get; set; }

        /// <summary>
        /// Gets or sets default caching policy to be applied to all new connections.
        /// </summary>
        public static CachingPolicy DefaultCachingPolicy { get; set; }

        /// <summary>
        /// Registers the provider factory 
        /// </summary>
        public static void RegisterProvider()
        {
            DbProviderFactoryBase.RegisterProvider("EF Caching Data Provider", "EFCachingProvider", "Caching Data Provider", typeof(EFCachingProviderFactory));
        }
    }
}
