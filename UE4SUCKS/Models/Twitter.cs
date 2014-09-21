using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace UE4SUCKS.Models
{
	static class Twitter
	{
		static readonly string credentialsUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";
		static readonly string tweetUrl = "https://api.twitter.com/1.1/statuses/update.json";

		static HttpClient client = Authorizer.GetClient();

		public static void ReAuthorize()
		{
			client = Authorizer.GetClient();
		}

		public static IObservable<string> UserName()
		{
			return client.GetAsync(credentialsUrl)
				.ToObservable()
				.SelectMany(x => x.Content.ReadAsStringAsync())
				.Select(x => DynamicJson.Parse(x).name).Cast<string>();
		}

		public static IObservable<int> Tweet(string tweet)
		{
			var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("status", tweet) });
			return client.PostAsync(tweetUrl, content)
				.ToObservable()
				.SelectMany(x => x.Content.ReadAsStringAsync())
				.Select(DynamicJson.Parse)
				.Select(x => (int)x.id);
		}
	}
}
