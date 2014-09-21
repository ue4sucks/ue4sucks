using System.Windows;

using UE4SUCKS.ViewModels;

namespace UE4SUCKS.Views
{
	public partial class AuthorizeWindow : Window
	{
		public string PinCode { get { return (DataContext as AuthorizeViewModel).Input; } }

		public AuthorizeWindow()
		{
			InitializeComponent();
		}

		void Input(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
