using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatterbox.Client.Helpers
{
    /// <inheritdoc/>
    internal class AsyncCommand : IAsyncCommand
    {
        /// <summary>
        /// Gets the <see cref="IDispatcher"/> which encapsulates the access to the <see cref="System.Windows.Threading.Dispatcher"/>.
        /// </summary>
        private readonly IDispatcher dispatcher;

        /// <summary>
        /// Gets the <see cref="ILogger"/>, which will be used by this class for writing log messages.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Gets or sets the <see cref="Func{TResult}"/> behind the <see cref="CanExecuteFunc"/>-property.
        /// </summary>
        private Func<bool> canExecute;

        /// <summary>
        /// Gets or sets the <see cref="Func{TResult}"/> behind the <see cref="ExecuteFunc"/>-property.
        /// </summary>
        private Func<Task> execute;

        /// <summary>
        /// Gets or sets if the <see cref="execute"/>-function is running or not.
        /// </summary>
        private bool isExecuting;

        /// <inheritdoc/>
        public Func<bool> CanExecuteFunc
        {
            get { return canExecute; }
            set
            {
                canExecute = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <inheritdoc/>
        public Func<Task> ExecuteFunc
        {
            get { return execute; }
            set
            {
                execute = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="AsyncCommand"/>.
        /// </summary>
        /// <param name="dispatcher">This instance of <see cref="IDispatcher"/> will become the <see cref="AsyncCommand"/>'s <see cref="dispatcher"/>.</param>
        /// <param name="logger">This <see cref="ILogger"/> will become the <see cref="AsyncCommand"/>'s <see cref="logger"/>.</param>
        public AsyncCommand(IDispatcher dispatcher, ILogger<AsyncCommand> logger)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute()
        {
            return execute != null && !isExecuting && (canExecute?.Invoke() ?? true);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            CanExecuteChanged = null;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                logger.LogDebug("Executing");
                try
                {
                    isExecuting = true;
                    await execute().ConfigureAwait(false);
                    logger.LogDebug("Executed");
                }
                finally
                {
                    isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        /// <inheritdoc/>
        public void RaiseCanExecuteChanged()
        {
            logger.LogDebug("Raising CanExecuteChanged event");
            dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        #region Explicit implementations

        /// <inheritdoc/>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <inheritdoc/>
        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(logger);
        }

        #endregion
    }
}