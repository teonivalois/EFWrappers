using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.EntityClient;

namespace EFTracingProvider
{
    /// <summary>
    /// Utility methods for the Entity Framework Tracing Provider.
    /// </summary>
    public static class EFTracingProviderUtils
    {
        /// <summary>
        /// Creates a traced entity connection wrapping the original entity connection string.
        /// </summary>
        /// <param name="entityConnectionString">The original entity connection string. This may also be a single word, e.g., "MyEntities", in which case it is translated into "name=MyEntities" and looked up in the application configuration.</param>
        /// <returns>An entity connection.</returns>
        public static EntityConnection CreateTracedEntityConnection(string entityConnectionString)
        {
            EFTracingProviderFactory.Register();
            return EFProviderWrapperToolkit.EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(entityConnectionString, EFTracingProviderFactory.InvariantName);
        }
    }
}
