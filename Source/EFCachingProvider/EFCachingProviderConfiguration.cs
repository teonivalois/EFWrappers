// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Configuration;
using EFCachingProvider.Caching;
using EFProviderWrapperToolkit;
using System;

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
            string customPolicyTypeName = ConfigurationManager.AppSettings["EFCachingProvider.CachingPolicyType"];
            if (!string.IsNullOrEmpty(customPolicyTypeName)) {
                Type customPolicyType = null;
                try 
                {
                    customPolicyType = Type.GetType(customPolicyTypeName);
                } 
                catch 
                {
                    throw new Exception("Failed to instantiate Caching Policy for type: " + customPolicyTypeName);
                }

                if (customPolicyType == null || customPolicyType.IsAssignableFrom(typeof(CachingPolicy))) 
                {
                    throw new Exception("Provided Caching Policy is not an instance of: " + typeof(CachingPolicy).FullName);
                }

                DefaultCachingPolicy = (CachingPolicy)Activator.CreateInstance(customPolicyType);
            }

            DefaultCache = null;
            string customCacheProviderName = ConfigurationManager.AppSettings["EFCachingProvider.CachingProviderType"];
            if (!string.IsNullOrEmpty(customCacheProviderName)) {
                Type customProviderType = Type.GetType(customCacheProviderName);
                try 
                {
                    customProviderType = Type.GetType(customCacheProviderName);
                } 
                catch 
                {
                    throw new Exception("Failed to instantiate Caching Provider for type: " + customCacheProviderName);
                }

                if (customProviderType == null || customProviderType.IsAssignableFrom(typeof(ICache))) 
                {
                    throw new Exception("Provided Caching Provider is not an instance of: " + typeof(ICache).FullName);
                }

                DefaultCache = (ICache)Activator.CreateInstance(customProviderType);
            }

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
