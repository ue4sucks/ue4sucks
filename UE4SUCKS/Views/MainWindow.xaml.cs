using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;

using Microsoft.Win32;
using UE4SUCKS.Models;
using UE4SUCKS.ViewModels;

namespace UE4SUCKS.Views
{
	public partial class MainWindow : Window
	{
		static readonly string filter = "UE4Editor|*.exe|全てのファイル|*.*";

		readonly MainViewModel viewModel;

		public MainWindow()
		{
			InitializeComponent();
			viewModel = DataContext as MainViewModel;
		}

		void Authorize(object sender, RoutedEventArgs e)
		{
			var authorizer = new Authorizer();
			authorizer.Request()
				.Subscribe(x => Process.Start(x));

			var window = new AuthorizeWindow();
			if (window.ShowDialog() == true)
			{
				authorizer.Authorize(window.PinCode)
					.SelectMany(_ => Twitter.UserName())
					.Do(x => Setting.Name = x)
					.Do(_ => Twitter.ReAuthorize())
					.Subscribe(x => Dispatcher.Invoke(() => viewModel.Authorize(x)));
			}
		}

		void Watch(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog() { Filter = filter };
			if (dialog.ShowDialog() == true)
			{
				Setting.ProcessUri = new Uri(dialog.FileName);
				viewModel.Watch(dialog.FileName);
			}
		}
	}
}
