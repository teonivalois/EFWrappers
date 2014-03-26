// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Data.Common;
using EFProviderWrapperToolkit;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;

namespace EFCachingProvider
{
    /// <summary>
    /// Implementation of <see cref="DbProviderServices"/> for EFCachingProvider.
    /// </summary>
    internal class EFCachingProviderServices : DbProviderServicesBase
    {
        /// <summary>
        /// Initializes static members of the EFCachingProviderServices class.
        /// </summary>
        static EFCachingProviderServices()
        {
            Instance = new EFCachingProviderServices();
        }

        /// <summary>
        /// Prevents a default instance of the EFCachingProviderServices class from being created.
        /// </summary>
        private EFCachingProviderServices()
        {
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="EFCachingProviderServices"/>.
        /// </summary>
        /// <value>The singleton instance.</value>
        public static EFCachingProviderServices Instance { get; private set; }

        /// <summary>
        /// Gets the name default of the wrapped provider.
        /// </summary>
        /// <returns>Name of the default wrapped provider.</returns>
        protected override string DefaultWrappedProviderName
        {
            get { return EFCachingProviderConfiguration.DefaultWrappedFactory; }
        }

        /// <summary>
        /// Gets the provider invariant iname.
        /// </summary>
        /// <returns>Provider invariant name.</returns>
        protected override string ProviderInvariantName
        {
            get { return "EFCachingProvider"; }
        }

        /// <summary>
        /// Creates the command definition wrapper.
        /// </summary>
        /// <param name="wrappedCommandDefinition">The wrapped command definition.</param>
        /// <param name="commandTree">The command tree.</param>
        /// <returns>
        /// The <see cref="DbCommandDefinitionWrapper"/> object.
        /// </returns>
        public override DbCommandDefinitionWrapper CreateCommandDefinitionWrapper(DbCommandDefinition wrappedCommandDefinition, DbCommandTree commandTree)
        {
            return new EFCachingCommandDefinition(wrappedCommandDefinition, commandTree);
        }
    }
}

