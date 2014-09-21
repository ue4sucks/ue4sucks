using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace UE4SUCKS
{
	[DataContract]
	sealed class ProcessResult
	{
		[DataMember]
		readonly bool isAbort;
		public bool IsAbort { get { return isAbort; } }

		[DataMember]
		readonly TimeSpan time;
		public TimeSpan Time { get { return time; } }

		[DataMember]
		readonly DateTime date;
		public DateTime Date { get { return date; } }

		public ProcessResult(bool isAbort, TimeSpan time, DateTime date)
		{
			this.isAbort = isAbort;
			this.time = time;
			this.date = date;
		}
	}

	sealed class ProcessLog
	{
		static readonly string path = "./clash_log.json";
		static readonly List<ProcessResult> results = new List<ProcessResult>();

		static ProcessLog()
		{
			using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read))
			{
				if (stream.Length == 0)
					return;

				var serializer = new DataContractJsonSerializer(typeof(ProcessResult[]));
				var values = (ProcessResult[])serializer.ReadObject(stream);
				results.AddRange(values);
			}
		}

		public static void Log(ProcessResult result)
		{
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Write))
			{
				results.Add(result);
				var serializer = new DataContractJsonSerializer(typeof(List<ProcessResult>));
				serializer.WriteObject(stream, results);
				stream.Flush();
			}
		}

		public static int Count()
		{
			return results
				.Where(x => x.IsAbort)
				.Count();
		}

		static bool IsSameDay(DateTime left, DateTime right)
		{
			return left.Year == right.Year
				&& left.Month == right.Month
				&& left.Day == right.Day;
		}

		public static int Count(DateTime date)
		{
			return results
				.Where(x => x.IsAbort)
				.Where(x => IsSameDay(x.Date, date))
				.Count();
		}
	}
}
