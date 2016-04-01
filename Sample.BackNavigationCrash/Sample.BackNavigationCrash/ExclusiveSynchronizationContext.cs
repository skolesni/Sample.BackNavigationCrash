
namespace Sample.BackNavigationCrash
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Implementation of an exclusive synchronization context.
    /// </summary>
    internal class ExclusiveSynchronizationContext : SynchronizationContext, IDisposable
    {
        #region Fields

        /// <summary>
        /// The work items waiting.
        /// </summary>
        private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

        /// <summary>
        /// The work items.
        /// </summary>
        private readonly Queue<Tuple<SendOrPostCallback, object>> workItems =
            new Queue<Tuple<SendOrPostCallback, object>>();

        /// <summary>
        /// A value indicating whether the work is done.
        /// </summary>
        private bool isWorkDone;

        /// <summary>
        /// A value indicating whether this instance has been disposed.
        /// </summary>
        private bool isDisposed;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the inner exception.
        /// </summary>
        /// <value>The inner exception.</value>
        public Exception InnerException { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Dispatches a synchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        /// <exception cref="System.NotSupportedException">We cannot send to our same thread</exception>
        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("We cannot send to our same thread");
        }

        /// <summary>
        /// Dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="d">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            lock (this.workItems)
            {
                this.workItems.Enqueue(Tuple.Create(d, state));
            }

            this.workItemsWaiting.Set();
        }

        /// <summary>
        /// Begins the message loop.
        /// </summary>
        /// <exception cref="System.AggregateException">AsyncHelpers.Run method threw an exception.</exception>
        public void BeginMessageLoop()
        {
            while (!this.isWorkDone)
            {
                Tuple<SendOrPostCallback, object> task = null;
                lock (this.workItems)
                {
                    if (this.workItems.Count > 0)
                    {
                        task = this.workItems.Dequeue();
                    }
                }

                if (task == null)
                {
                    this.workItemsWaiting.WaitOne();
                }
                else
                {
                    task.Item1(task.Item2);

                    // The method threw an exception
                    if (this.InnerException != null)
                    {
                        throw new AggregateException("AsyncHelpers.Run method threw an exception.", this.InnerException);
                    }
                }
            }
        }

        /// <summary>
        /// Ends the message loop.
        /// </summary>
        public void EndMessageLoop()
        {
            this.Post(_ => this.isWorkDone = true, null);
        }

        /// <summary>
        /// Creates a copy of the synchronization context.
        /// </summary>
        /// <returns>A new <see cref="T:System.Threading.SynchronizationContext" /> object.</returns>
        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.workItemsWaiting.Dispose();
            }

            this.isDisposed = true;
        }

        #endregion Methods
    }
}
