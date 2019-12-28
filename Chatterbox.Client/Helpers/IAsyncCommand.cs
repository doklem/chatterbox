using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chatterbox.Client.Helpers
{
    /// <summary>
    /// An asynchronous version of the <see cref="ICommand"/>. It's inspired by the solution from https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </summary>
    public interface IAsyncCommand : ICommand, IDisposable
    {
        /// <summary>
        /// Gets or sets the <see cref="Func{TResult}"/>, which tells if the command can be executed or not. It's optional, so <code>null</code> values are possible.
        /// </summary>
        Func<bool> CanExecuteFunc { get; set; }

        /// <summary>
        /// Gets or sets the actual <see cref="Func{TResult}"/>, which will be executed by the command.
        /// </summary>
        Func<Task> ExecuteFunc { get; set; }

        /// <summary>
        /// Executes the actual <see cref="Func{TResult}"/> within the command in an asynchronous mannner.
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();

        /// <summary>
        /// The inputless version of <see cref="ICommand.CanExecute(object)"/>.
        /// </summary>
        /// <returns>Returns <code>true</code> if the execution is possible, else <code>false</code>.</returns>
        bool CanExecute();

        /// <summary>
        /// Raises a <see cref="ICommand.CanExecuteChanged"/>-event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
