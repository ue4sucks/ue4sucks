using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace UE4SUCKS.Models
{
	sealed class Watcher
	{
		static readonly string tweetFormat =
@"Unreal Engine 4が落ちました.
本日{0}回目, 通算{1}回目, 起動時間は{2}でした.

by @UE4SUCKS #UE4SUCKS";

		public static bool IsWatched { get { return Setting.ProcessUri != null; } }

		readonly Process process;
		readonly Subject<Process> subject = new Subject<Process>();

		public Watcher()
		{
			var url = Setting.ProcessUri.ToString();
			process = Process.Start(url);
			process.EnableRaisingEvents = true;
			process.Exited += (sender, _) => subject.OnNext(sender as Process);

			Watch().Subscribe(ProcessLog.Log);
		}

		ProcessResult CreateResult(Process process)
		{
			var isAbort = process.ExitCode != 0;
			var time = process.ExitTime - process.StartTime;
			var date = process.ExitTime;
			return new ProcessResult(isAbort, time, date);
		}

		public IObservable<ProcessResult> Watch()
		{
			return subject.Select(CreateResult);
		}

		string Tweet(DateTime start, DateTime exit)
		{
			var today = ProcessLog.Count(exit);
			var all = ProcessLog.Count();
			var time = (exit - start).PrettyPrint();
			return String.Format(tweetFormat, today, all, time);
		}

		public IObservable<int> TweetIfAbort()
		{
			return Watch()
				.Where(x => x.IsAbort && Authorizer.IsAuthorized)
				.Select(_ => Tweet(process.StartTime, process.ExitTime))
				.SelectMany(x => Twitter.Tweet(x));
		}

		public void Reboot()
		{
			process.Start();
		}
	}
}
