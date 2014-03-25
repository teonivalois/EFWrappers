// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Globalization;
using System.Text;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace EFTracingProvider
{
    /// <summary>
    /// Arguments to <see cref="EFTracingConnection.CommandExecuting" />, <see cref="EFTracingConnection.CommandFinished" /> 
    /// and <see cref="EFTracingConnection.CommandFailed" /> events.
    /// </summary>
    public class CommandExecutionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the CommandExecutionEventArgs class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="method">The method.</param>
        internal CommandExecutionEventArgs(EFTracingCommand command, string method)
        {
            this.CommandId = command.CommandID;
            this.Command = command;
            this.CommandTree = command.Definition.CommandTree;
            this.Method = method;
        }

        /// <summary>
        /// Gets the command ID.
        /// </summary>
        /// <value>The command ID.</value>
        public int CommandId { get; private set; }

        /// <summary>
        /// Gets the command object.
        /// </summary>
        /// <value>The command.</value>
        public DbCommand Command { get; private set; }

        /// <summary>
        /// Gets the command tree.
        /// </summary>
        /// <value>The command tree.</value>
        public DbCommandTree CommandTree { get; private set; }

        /// <summary>
        /// Gets the method which caused command execution (ExecuteScalar, ExecuteQuery, ExecuteNonQuery).
        /// </summary>
        /// <value>The method name.</value>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the execution status.
        /// </summary>
        /// <value>Execution status.</value>
        public CommandExecutionStatus Status { get; internal set; }

        /// <summary>
        /// Gets the command result.
        /// </summary>
        /// <value>The command result.</value>
        public object Result { get; internal set; }

        /// <summary>
        /// Gets the time it took to execute the command.
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Returns textual dump of the command suitable for putting in a log file.
        /// </summary>
        /// <returns>Textual dump of the command including command text and parameters, 
        /// suitable for putting in a log file.</returns>
        public string ToTraceString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Command.CommandText);

            foreach (DbParameter par in this.Command.Parameters)
            {
                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "\r\n-- {0} (dbtype={1}, size={4}, direction={2}) = {3}", 
                    par.ParameterName, 
                    par.DbType, 
                    par.Direction, 
                    GetValueText(par.Value), 
                    par.Size);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Condenses a <see cref="CommandExecutionEventArgs"/> notification into a single line of text.
        /// </summary>
        /// <returns>The notification, as a single line of text.</returns>
        public string ToFlattenedTraceString()
        {
            Contract.Ensures(Contract.Result<string>() != null);

            string ret;
            switch (this.Status)
            {
                case CommandExecutionStatus.Executing:
                    ret = "Executing " + this.CommandId + ": " + this.Command.CommandText;
                    var parameters = this.Command.Parameters.Cast<DbParameter>();
                    if (parameters.Any())
                    {
                        ret += " { " + string.Join(", ", parameters.Select(x => x.ParameterName + "=[" + x.DbType + "," + x.Size + "," + x.Direction + "]" + ((x.Value == null || x.Value == DBNull.Value) ? "NULL" : x.Value))) + " }";
                    }
                    break;
                case CommandExecutionStatus.Finished:
                    string resultInfo;
                    if (this.Method == "ExecuteNonQuery")
                    {
                        // Insert/delete operation
                        resultInfo = this.Result + " rows affected";
                    }
                    else if (this.Result is DbDataReader)
                    {
                        // Table result
                        var dataReader = (DbDataReader)this.Result;
                        resultInfo = "[DbDataReader(" + string.Join(", ", Enumerable.Range(0, dataReader.FieldCount).Select(i => dataReader.GetName(i) + ":" + dataReader.GetDataTypeName(i))) + ")]";
                    }
                    else
                    {
                        // Scalar result
                        resultInfo = "[" + this.Result.GetType() + "] " + this.Result;
                    }

                    ret = "Finished " + this.CommandId + " in " + this.Duration + ": " + resultInfo;
                    break;
                case CommandExecutionStatus.Failed:
                    ret = "Failed " + this.CommandId + ": " + string.Join(" -- ", SelfAndInnerExceptions((Exception)this.Result).Select(x => "[" + x.GetType() + "] " + x.Message));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown DbCommand execution status.");
            }

            return CompressWhitespace(ret);
        }

        private static readonly Regex WhitespaceRegex = new Regex(@"\s+");

        private static string CompressWhitespace(string source)
        {
            return WhitespaceRegex.Replace(source, " ");
        }

        private static IEnumerable<Exception> SelfAndInnerExceptions(Exception exception)
        {
            var ex = exception;
            while (ex != null)
            {
                yield return ex;
                ex = ex.InnerException;
            }
        }

        private static string GetValueText(object value)
        {
            if (value == null || value is DBNull)
            {
                return "null";
            }
            else if (value is string)
            {
                return "\"" + value + "\"";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
