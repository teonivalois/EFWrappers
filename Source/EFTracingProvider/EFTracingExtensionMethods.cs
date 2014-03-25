// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Data.Common;
using System.Data.Objects;
using System.IO;
using EFProviderWrapperToolkit;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Diagnostics;
using System.Data.EntityClient;

namespace EFTracingProvider
{
    /// <summary>
    /// Extension methods for EFTracingProvider.
    /// </summary>
    public static class EFTracingExtensionMethods
    {
        /// <summary>
        /// Gets all instances of <see cref="EFTracingConnection" /> from <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        /// <returns>Instances of <see cref="EFTracingConnection"/>.</returns>
        public static IEnumerable<EFTracingConnection> GetTracingConnections(this DbConnection connection)
        {
            Contract.Requires(connection != null);

            return connection.SelfAndWrappedConnections().OfType<EFTracingConnection>();
        }

        /// <summary>
        /// Sets the trace source for all instances of <see cref="EFTracingConnection"/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="traceSource">The trace source to which to trace SQL command activity.</param>
        public static void SetTraceSource(this DbConnection connection, TraceSource traceSource)
        {
            Contract.Requires(connection != null);
            Contract.Requires(traceSource != null);

            foreach (var tracingConnection in connection.GetTracingConnections())
            {
                tracingConnection.CommandExecuting += (_, e) => traceSource.TraceInformation(e.ToFlattenedTraceString());
                tracingConnection.CommandFinished += (_, e) => traceSource.TraceInformation(e.ToFlattenedTraceString());
                tracingConnection.CommandFailed += (_, e) => traceSource.TraceEvent(TraceEventType.Error, 0, e.ToFlattenedTraceString());
            }
        }

        /// <summary>
        /// Enables SQL command tracing for all instances of <see cref="EFTracingConnection"/> in the object context.
        /// </summary>
        /// <param name="context">The object context.</param>
        /// <param name="traceSourceName">The name of the trace source; if not specified, it defaults to "EntityFramework." + <see cref="ObjectContext.DefaultContainerName"/>.</param>
        public static void EnableTracing(this ObjectContext context, string traceSourceName = null)
        {
            Contract.Requires(context != null);
            Contract.Assume(context.Connection != null);
            
            context.Connection.SetTraceSource(new TraceSource(traceSourceName ?? "EntityFramework." + context.DefaultContainerName));
        }
    }
}
