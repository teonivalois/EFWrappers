﻿using System;
using System.IO;
using EFCachingProvider;
using EFCachingProvider.Caching;
using EFProviderWrapperToolkit;
using EFTracingProvider;
using System.Data.Entity.Infrastructure;

namespace EFProviderWrapperDemo
{
    public partial class NorthwindEFEntities
    {
        private TextWriter logOutput;

        public NorthwindEFEntities(string connectionString)
            : base(EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(
                    connectionString,
                    "EFTracingProvider",
                    "EFCachingProvider"
            ), true)
        {
        }

        #region Tracing Extensions

        private EFTracingConnection TracingConnection
        {
            get { return ((IObjectContextAdapter)this).ObjectContext.UnwrapConnection<EFTracingConnection>(); }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandExecuting
        {
            add { this.TracingConnection.CommandExecuting += value; }
            remove { this.TracingConnection.CommandExecuting -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFinished
        {
            add { this.TracingConnection.CommandFinished += value; }
            remove { this.TracingConnection.CommandFinished -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFailed
        {
            add { this.TracingConnection.CommandFailed += value; }
            remove { this.TracingConnection.CommandFailed -= value; }
        }

        private void AppendToLog(object sender, CommandExecutionEventArgs e)
        {
            if (this.logOutput != null)
            {
                this.logOutput.WriteLine(e.ToTraceString().TrimEnd());
                this.logOutput.WriteLine();
            }
        }

        public TextWriter Log
        {
            get { return this.logOutput; }
            set
            {
                if ((this.logOutput != null) != (value != null))
                {
                    if (value == null)
                    {
                        CommandExecuting -= AppendToLog;
                    }
                    else
                    {
                        CommandExecuting += AppendToLog;
                    }
                }

                this.logOutput = value;
            }
        }


        #endregion

        #region Caching Extensions

        private EFCachingConnection CachingConnection
        {
            get { return ((IObjectContextAdapter)this).ObjectContext.UnwrapConnection<EFCachingConnection>(); }
        }

        public ICache Cache
        {
            get { return CachingConnection.Cache; }
            set { CachingConnection.Cache = value; }
        }

        public CachingPolicy CachingPolicy
        {
            get { return CachingConnection.CachingPolicy; }
            set { CachingConnection.CachingPolicy = value; }
        }

        #endregion
    }

}
