// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Diagnostics.Contracts;
using System.Linq;

namespace EFProviderWrapperToolkit
{
    /// <summary>
    /// Common implementation of the <see cref="DbProviderFactory"/> methods.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class DbProviderFactoryBase : DbProviderFactory
    {
        private DbProviderServices services;

        /// <summary>
        /// Initializes a new instance of the DbProviderFactoryBase class.
        /// </summary>
        /// <param name="providerServices">The provider services.</param>
        protected DbProviderFactoryBase(DbProviderServices providerServices)
        {
            this.services = providerServices;
        }

        /// <summary>
        /// Specifies whether the specific <see cref="T:System.Data.Common.DbProviderFactory"/> supports the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class.
        /// </summary>
        /// <value></value>
        /// <returns>true if the instance of the <see cref="T:System.Data.Common.DbProviderFactory"/> supports the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class; otherwise false.
        /// </returns>
        public override bool CanCreateDataSourceEnumerator
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Registers the specified <see cref="DbProviderFactory"/> type, overwriting any existing registration.
        /// </summary>
        /// <param name="name">The friendly name of the provider factory.</param>
        /// <param name="invariantName">The invariant name of the provider factory. This must be unique.</param>
        /// <param name="description">The description of the provider factory.</param>
        /// <param name="factoryType">The type of the provider factory. This must be derived from <see cref="DbProviderFactory"/>.</param>
        /// <returns><c>true</c> if the registration succeeded; <c>false</c> if the registration failed.</returns>
        public static bool RegisterProvider(string name, string invariantName, string description, Type factoryType)
        {
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrEmpty(invariantName));
            Contract.Requires(description != null);
            Contract.Requires(factoryType != null);

            var systemData = ConfigurationManager.GetSection("system.data") as DataSet;
            if (systemData == null)
                return false;
            if (!systemData.Tables.Contains("DbProviderFactories"))
                return false;
            var dbProviders = systemData.Tables["DbProviderFactories"];
            if (dbProviders == null)
                return false;
            var existing = dbProviders.Rows.Cast<DataRow>().FirstOrDefault(x => Convert.ToString(x["InvariantName"]) == invariantName);
            if (existing != null)
                dbProviders.Rows.Remove(existing);
            var row = dbProviders.NewRow();
            row["Name"] = name;
            row["InvariantName"] = invariantName;
            row["Description"] = description;
            row["AssemblyQualifiedName"] = factoryType.AssemblyQualifiedName;
            dbProviders.Rows.Add(row);
            return true;
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbCommandBuilder"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbCommandBuilder"/>.
        /// </returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbConnectionStringBuilder"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbConnectionStringBuilder"/>.
        /// </returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbDataAdapter"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbDataAdapter"/>.
        /// </returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbDataSourceEnumerator"/>.
        /// </returns>
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbParameter"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbParameter"/>.
        /// </returns>
        public override DbParameter CreateParameter()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbConnection"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbConnection"/>.
        /// </returns>
        public abstract override DbConnection CreateConnection();
    }
}
