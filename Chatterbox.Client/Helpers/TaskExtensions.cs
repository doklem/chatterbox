using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Chatterbox.Client.Helpers
{
    /// <summary>
    /// This static class extends the <see cref="Task"/>-<see cref="Type"/>.
    /// </summary>
    internal static class TaskExtensions
    {
        /// <summary>
        /// Encapsulates the execution of <see cref="Task"/> within a "async void" method. It should only be used, if there is no other way.
        /// If it produces an <see cref="Exception"/> it will be handeled by the <see cref="FallbackExceptionHandler"/>.
        /// </summary>
        /// <param name="task">This <see cref="Task"/> will be executed within this method.</param>
        /// <param name="logger">This instance of <see cref="ILogger"/> will be used for writing error log messages. It's optional.</param>
        /// <param name="continueOnCapturedContext">This flag will be used to configure the <paramref name="task"/>'s <see cref="System.Runtime.CompilerServices.TaskAwaiter"/>.
        /// The default value is <code>false</code>.</param>
        public static async void FireAndForgetSafeAsync(this Task task, ILogger logger = null, bool continueOnCapturedContext = false)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error happend");
                FallbackExceptionHandler.ShowError(ex);
            }
        }
    }
}
