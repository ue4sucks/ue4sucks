using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;

namespace UE4SUCKS.Views.Control
{
	static class DocumentBinding
	{
		static FlowDocument document;

		public static ObservableCollection<Block> GetBlocks(DependencyObject obj)
		{
			return (ObservableCollection<Block>)obj.GetValue(BlocksProperty);
		}

		public static void SetBlocks(DependencyObject obj, ObservableCollection<Block> value)
		{
			obj.SetValue(BlocksProperty, value);
		}

		public static readonly DependencyProperty BlocksProperty =
				DependencyProperty.RegisterAttached("Blocks", typeof(ObservableCollection<Block>), typeof(DocumentBinding), new PropertyMetadata(new ObservableCollection<Block>(), OnBlocksChanged));

		static void OnBlocksChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			document = dp as FlowDocument;
			if (document == null)
				return;

			(e.OldValue as ObservableCollection<Block>).CollectionChanged -= OnBlockCollectionChanged;
			(e.NewValue as ObservableCollection<Block>).CollectionChanged += OnBlockCollectionChanged;
		}

		static void OnBlockCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			document.Blocks.AddRange(e.NewItems);
		}
	}
}
