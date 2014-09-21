using System;

namespace UE4SUCKS
{
	static class Helper
	{
		public static string PrettyPrint(this TimeSpan time)
		{
			if (time.Days > 0)
				return String.Format("{0}日", time.Days);

			var text = "";
			if (time.Hours > 0)
				text += String.Format("{0}時間", time.Hours);
			if (time.Minutes > 0)
				text += String.Format("{0}分", time.Minutes);
			return text + String.Format("{0}秒", time.Seconds);
		}
	}
}
