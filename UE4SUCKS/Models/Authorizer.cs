using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Security.Cryptography;

using AsyncOAuth;

namespace UE4SUCKS.Models
{
	sealed class Authorizer
	{
		static readonly string key = "WD7mLNbZcsSBAfE880CpkgLmQ";
		static readonly string secret = "styptp0fbtRZT9ml57k9BUEASleF2iwtQTSKEVPUej31bf87SP";

		static readonly string requestTokenUrl = "https://api.twitter.com/oauth/request_token";
		static readonly string accessTokenUrl = "https://api.twitter.com/oauth/access_token";
		static readonly string authorizeUrl = "https://api.twitter.com/oauth/authorize";

		public static bool IsAuthorized { get { return Setting.AccessToken != null; } }

		readonly OAuthAuthorizer authorizer = new OAuthAuthorizer(key, secret);
		RequestToken requestToken;

		static Authorizer()
		{
			OAuthUtility.ComputeHash = (key, buffer) =>
			{
				using (var hmac = new HMACSHA1(key))
				{
					return hmac.ComputeHash(buffer);
				}
			};
		}

		public IObservable<string> Request()
		{
			return authorizer.GetRequestToken(requestTokenUrl)
				.ToObservable()
				.Do(x => requestToken = x.Token)
				.Select(x => authorizer.BuildAuthorizeUrl(authorizeUrl, x.Token));
		}

		public IObservable<HttpClient> Authorize(string pincode)
		{
			return authorizer.GetAccessToken(accessTokenUrl, requestToken, pincode)
				.ToObservable()
				.Do(x => Setting.AccessToken = x.Token)
				.Select(x => OAuthUtility.CreateOAuthClient(key, secret, x.Token));
		}

		public static HttpClient GetClient()
		{
			if (!IsAuthorized)
				throw new InvalidOperationException();

			var token = Setting.AccessToken;
			return OAuthUtility.CreateOAuthClient(key, secret, token);
		}
	}
}
