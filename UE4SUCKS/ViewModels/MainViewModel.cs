using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using UE4SUCKS.Models;

namespace UE4SUCKS.ViewModels
{
	sealed class MainViewModel : ViewModel
	{
		#region TwitterAuthLabel

		string twitterAuthLabel = "Twitter認証が必要です";
		public string TwitterAuthLabel
		{
			get { return twitterAuthLabel; }
			set
			{
				twitterAuthLabel = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region ProcessAppointLabel

		string processAppointLabel = "UE4Editor.exeの指定が必要です";
		public string ProcessAppointLabel
		{
			get { return processAppointLabel; }
			set
			{
				processAppointLabel = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region TwitterAuthBrush

		Brush twitterAuthBrush = Brushes.Red;
		public Brush TwitterAuthBrush
		{
			get { return twitterAuthBrush; }
			set
			{
				twitterAuthBrush = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region ProcessAppointBrush

		Brush processAppointBrush = Brushes.Red;
		public Brush ProcessAppointBrush
		{
			get { return processAppointBrush; }
			set
			{
				processAppointBrush = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region TwitterAuthButton

		string twitterAuthButton = "認証を行う";
		public string TwitterAuthButton
		{
			get { return twitterAuthButton; }
			set
			{
				twitterAuthButton = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region ProcessAppointButton

		string processAppointButton = "プロセスを指定する";
		public string ProcessAppointButton
		{
			get { return processAppointButton; }
			set
			{
				processAppointButton = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region ProcessAppointTooltip

		string processAppointTooltip = "";
		public string ProcessAppointTooltip
		{
			get { return processAppointTooltip; }
			set
			{
				processAppointTooltip = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region HeaderVisibility

		Visibility headerVisibility = Maximize(Setting.IsMaximized);
		public Visibility HeaderVisibility
		{
			get { return headerVisibility; }
			set
			{
				headerVisibility = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region IsProcessWatched

		bool isProcessWatched = false;
		public bool IsProcessWatched
		{
			get { return isProcessWatched; }
			set
			{
				isProcessWatched = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		#region IsAutomatized

		public bool IsAutomatized
		{
			get { return Setting.IsAutomatized; }
			set
			{
				Setting.IsAutomatized = value;
				RaisePropertyChanged();

				if (IsAutomatized)
					Automatize();
				else
					Manualize();
			}
		}

		#endregion

		#region IsMaximized

		public bool IsMaximized
		{
			get { return Setting.IsMaximized; }
			set
			{
				Setting.IsMaximized = value;
				RaisePropertyChanged();
				HeaderVisibility = Maximize(IsMaximized);
			}
		}

		#endregion

		#region IsForefronted

		public bool IsForefronted
		{
			get { return Setting.IsForefront; }
			set
			{
				Setting.IsForefront = value;
				RaisePropertyChanged();
			}
		}

		#endregion

		readonly ObservableCollection<Block> blocks = new ObservableCollection<Block>();
		public ObservableCollection<Block> Blocks { get { return blocks; } }

		static int count = 1;
		Watcher watcher;
		IDisposable automatize;

		static Visibility Maximize(bool isMaximize)
		{
			return isMaximize ? Visibility.Collapsed : Visibility.Visible;
		}

		public MainViewModel()
		{
			if (Authorizer.IsAuthorized)
				Authorize(Setting.Name);
			if (Watcher.IsWatched)
				Watch(Setting.ProcessUri.ToString());
		}

		void Automatize()
		{
			automatize = watcher.Watch()
				.Where(x => x.IsAbort)
				.Do(_ => App.Current.Dispatcher.Invoke(() => LogReboot()))
				.Do(_ => IsProcessWatched = true)
				.Subscribe(_ => watcher.Reboot());
		}

		void Manualize()
		{
			automatize.Dispose();
		}

		public void Authorize(string name)
		{
			TwitterAuthLabel = String.Format("ようこそ{0}さん！", name);
			TwitterAuthBrush = Brushes.Blue;
			TwitterAuthButton = "別アカウントに紐付ける";
		}

		string ProcessLabelName(string url)
		{
			var directory = Path.GetFileName(Path.GetDirectoryName(url));
			if (directory == "")
				return url;
			return Path.Combine("..", directory, Path.GetFileName(url));
		}

		void LogAbort(ProcessResult result)
		{
			var paragraph = new Paragraph(new Run(String.Format("{0}: ", count++)));
			var text = new Run("UE4が落ちました   ") { Foreground = Brushes.Red };
			paragraph.Inlines.Add(new Bold(text));
			var time = String.Format("起動時間: {0} ", result.Time.PrettyPrint());
			paragraph.Inlines.Add(new Run(time));
			var number = String.Format("(本日{0}回目, 通算{1}回目)", ProcessLog.Count(result.Date), ProcessLog.Count());
			paragraph.Inlines.Add(new Run(number));
			Blocks.Add(paragraph);
		}

		void LogTweet()
		{
			var text = new Run(">: ツイートしました") { Foreground = Brushes.Green };
			var paragraph = new Paragraph(text);
			Blocks.Add(paragraph);
		}

		void LogReboot()
		{
			var text = new Run(">: UE4を再起動しました") { Foreground = Brushes.Blue };
			var paragraph = new Paragraph(text);
			Blocks.Add(paragraph);
		}

		public void Watch(string url)
		{
			ProcessAppointLabel = ProcessLabelName(url);
			ProcessAppointTooltip = url;
			ProcessAppointBrush = Brushes.Blue;
			ProcessAppointButton = "プロセスを再指定する";

			watcher = new Watcher();
			watcher.Watch()
				.Where(x => x.IsAbort)
				.Subscribe(x => App.Current.Dispatcher.Invoke(() => LogAbort(x)));
			watcher.TweetIfAbort()
				.Subscribe(_ => App.Current.Dispatcher.Invoke(() => LogTweet()));
			watcher.Watch()
				.Subscribe(_ => IsProcessWatched = false);
			IsProcessWatched = true;
			if (IsAutomatized)
				Automatize();
		}
	}
}
