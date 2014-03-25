// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Diagnostics.Contracts;

namespace EFProviderWrapperToolkit
{
    /// <summary>
    /// Extension methods for handing wrapped providers.
    /// </summary>
    public static class EFProviderWriterExtensionMethods
    {
        /// <summary>
        /// Returns the connection and its wrapped connections as an enumerable. This method is able to unwrap <see cref="EntityConnection"/> and <see cref="DbConnectionWrapper"/> wrappers.
        /// </summary>
        /// <param name="connection">The connection to enumerate. May not be <c>null</c>.</param>
        /// <returns>The connection and its wrapped connections as an enumerable.</returns>
        public static IEnumerable<DbConnection> SelfAndWrappedConnections(this DbConnection connection)
        {
            Contract.Requires(connection != null);

            var current = connection;
            while (true)
            {
                yield return current;

                var entityConnection = current as EntityConnection;
                if (entityConnection != null)
                {
                    current = entityConnection.StoreConnection;
                    continue;
                }

                var wrappedConnection = current as DbConnectionWrapper;
                if (wrappedConnection != null)
                {
                    current = wrappedConnection.WrappedConnection;
                    continue;
                }

                yield break;
            }
        }

        /// <summary>
        /// Returns the underlying "store" connection (that is, a database connection that is not an <see cref="EntityConnection"/> nor a <see cref="DbConnectionWrapper"/>).
        /// </summary>
        /// <param name="connection">The connection to examine. May not be <c>null</c>.</param>
        /// <returns>The store connection.</returns>
        public static DbConnection GetStoreConnection(this DbConnection connection)
        {
            Contract.Requires(connection != null);

            return connection.SelfAndWrappedConnections().Last();
        }

        /// <summary>
        /// Returns the underlying "store" connection (that is, a database connection that is not an <see cref="EntityConnection"/> nor a <see cref="DbConnectionWrapper"/>).
        /// </summary>
        /// <param name="context">The context whose connection is examined. May not be <c>null</c>.</param>
        /// <returns>The store connection.</returns>
        public static DbConnection GetStoreConnection(this ObjectContext context)
        {
            Contract.Requires(context != null);
            Contract.Assume(context.Connection != null);

            return context.Connection.GetStoreConnection();
        }

        /// <summary>
        /// Gets the underlying wrapper connection from the <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TConnection">Connection type.</typeparam>
        /// <param name="connection">The connection. May not be <c>null</c>.</param>
        /// <returns>Wrapper connection of a given type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type parameter must be specified explicitly.")]
        public static TConnection UnwrapConnection<TConnection>(this DbConnection connection) where TConnection : DbConnection
        {
            Contract.Requires(connection != null);

            return connection.SelfAndWrappedConnections().OfType<TConnection>().First();
        }

        /// <summary>
        /// Tries to get the underlying wrapper connection from the <see cref="DbConnection"/>.
        /// </summary>
        /// <typeparam name="TConnection">Connection type.</typeparam>
        /// <param name="connection">The connection. May not be <c>null</c>.</param>
        /// <param name="result">The result connection.</param>
        /// <returns>A value of true if the given connection type was found in the provider chain, false otherwise.</returns>
        public static bool TryUnwrapConnection<TConnection>(this DbConnection connection, out TConnection result) where TConnection : DbConnection
        {
            Contract.Requires(connection != null);
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn<TConnection>(out result) != null);

            result = connection.SelfAndWrappedConnections().OfType<TConnection>().FirstOrDefault();
            return (result != null);
        }

        /// <summary>
        /// Gets the underlying wrapper connection from the <see cref="ObjectContext"/>.
        /// </summary>
        /// <typeparam name="TConnection">Connection type.</typeparam>
        /// <param name="context">The object context. May not be <c>null</c>.</param>
        /// <returns>Wrapper connection of a given type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type parameter must be specified explicitly.")]
        public static TConnection UnwrapConnection<TConnection>(this ObjectContext context) where TConnection : DbConnection
        {
            Contract.Requires(context != null);
            Contract.Assume(context.Connection != null);

            return context.Connection.UnwrapConnection<TConnection>();
        }

        /// <summary>
        /// Tries to get the underlying wrapper connection from the <see cref="ObjectContext"/>.
        /// </summary>
        /// <typeparam name="TConnection">Connection type.</typeparam>
        /// <param name="context">The object context. May not be <c>null</c>.</param>
        /// <param name="result">The result connection.</param>
        /// <returns>A value of true if the given connection type was found in the provider chain, false otherwise.</returns>
        public static bool TryUnwrapConnection<TConnection>(this ObjectContext context, out TConnection result) where TConnection : DbConnection
        {
            Contract.Requires(context != null);
            Contract.Ensures(!Contract.Result<bool>() || Contract.ValueAtReturn<TConnection>(out result) != null);
            Contract.Assume(context.Connection != null);

            return context.Connection.TryUnwrapConnection<TConnection>(out result);
        }
    }
}
