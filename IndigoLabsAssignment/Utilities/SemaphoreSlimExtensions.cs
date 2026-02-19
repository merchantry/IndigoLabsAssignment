namespace IndigoLabsAssignment.Utilities
{
    public static class SemaphoreSlimExtensions
    {
        public static async Task<T> ExecuteWithLockAsync<T>(
            this SemaphoreSlim semaphore,
            Func<Task<T>> action
        )
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                return await action().ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
