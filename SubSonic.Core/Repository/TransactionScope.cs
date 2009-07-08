using System;
using System.Collections.Generic;
using System.Linq;
using SubSonic.DataProviders;
using SubSonic.Query;

namespace SubSonic.Repository
{
	public class TransactionScope : IDisposable
	{
		[ThreadStatic]
		private static Stack<TransactionScope> __instances;

		private readonly BatchQuery _batchQuery;
		private bool _disposed;

		public TransactionScope(IDataProvider provider)
		{
			_batchQuery = new BatchQuery(provider);

			if (__instances == null)
			{
				__instances = new Stack<TransactionScope>();
			}

			__instances.Push(this);
		}

		/// <summary>
		/// Retrieves the batch query of the current transaction.
		/// </summary>
		/// <returns>The batch query associated with the current transaction, null if no current transaction.</returns>
		public static BatchQuery GetCurrentBatchQuery()
		{
			var instance = __instances.First();
			if (instance == null)
			{
				return null;
			}

			return instance._batchQuery;
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		public void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// TODO: We should NOT execute the transaction in the dispose method. This is strictly for testing.
					_batchQuery.ExecuteTransaction();

					// Remove this instance from the stack
					__instances.Pop();

					_disposed = true;
				}
			}
		}
	}
}