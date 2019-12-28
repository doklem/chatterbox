using Chatterbox.Client.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;

namespace Chatterbox.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gets the name of the property, which will be used to sort the <see cref="Messages"/>.
        /// </summary>
        private static readonly string sortKey = nameof(MessageListItemBase.Time);

        /// <summary>
        /// Creates a new instance of <see cref="MainWindow"/>.
        /// </summary>
        /// <param name="viewModel">This instance of <see cref="MainViewModel"/> will become the <see cref="MainWindow"/>'s view model.</param>
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <inheritdoc/>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Messages.Items.SortDescriptions.Add(new SortDescription(sortKey, ListSortDirection.Descending));
        }
    }
}
