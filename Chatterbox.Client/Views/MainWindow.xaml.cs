using Chatterbox.Client.ViewModels;
using System;
using System.Windows;

namespace Chatterbox.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Creates a new instance of <see cref="MainWindow"/>.
        /// </summary>
        /// <param name="viewModel">This instance of <see cref="MainViewModel"/> will become the <see cref="MainWindow"/>'s view model.</param>
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
