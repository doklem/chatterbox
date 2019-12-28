using Chatterbox.Client.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Chatterbox.Client.Views
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        /// <summary>
        /// Gets the name of the property, which will be used to sort the <see cref="Messages"/>.
        /// </summary>
        private static readonly string sortKey = nameof(MessageListItemBase.Time);

        /// <summary>
        /// Creates a new instance of <see cref="ChatControl"/>.
        /// </summary>
        public ChatControl()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Messages.Items.SortDescriptions.Add(new SortDescription(sortKey, ListSortDirection.Descending));
        }
    }
}
