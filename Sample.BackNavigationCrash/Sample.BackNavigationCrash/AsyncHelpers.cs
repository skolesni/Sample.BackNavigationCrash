
namespace Sample.BackNavigationCrash
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Collection of async helpers.
    /// </summary>
    public static class AsyncHelpers
    {
        /// <summary>
        /// Executes an async method with void return value synchronously.
        /// </summary>
        /// <param name="task">The method to execute.</param>
        public static void RunSynchronously(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var exclusiveContext = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(exclusiveContext);

            exclusiveContext.Post(
                async _ =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception e)
                    {
                        exclusiveContext.InnerException = e;
                        throw;
                    }
                    finally
                    {
                        exclusiveContext.EndMessageLoop();
                    }
                },
                null);

            exclusiveContext.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Executes an async method with a return value synchronously.
        /// </summary>
        /// <typeparam name="T">The type of return value.</typeparam>
        /// <param name="task">The method to execute.</param>
        /// <returns>The task returning the return value.</returns>
        public static T RunSynchronously<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var exclusiveContext = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(exclusiveContext);

            var result = default(T);
            exclusiveContext.Post(
                async _ =>
                {
                    try
                    {
                        result = await task();
                    }
                    catch (Exception e)
                    {
                        exclusiveContext.InnerException = e;
                        throw;
                    }
                    finally
                    {
                        exclusiveContext.EndMessageLoop();
                    }
                },
                null);

            exclusiveContext.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
            return result;
        }
    }
}
