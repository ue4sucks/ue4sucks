using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using AsyncOAuth;

namespace UE4SUCKS
{
	static class Setting
	{
		[DataContract]
		sealed class Value
		{
			[DataMember]
			public string name;

			[DataMember]
			public AccessToken accessToken;

			[DataMember]
			public Uri processUri;

			[DataMember]
			public bool isAutomatized;

			[DataMember]
			public bool isMaximized;

			[DataMember]
			public bool isForefronted;
		}

		readonly static string path = "./setting.json";
		readonly static Value setting = new Value();

		static Setting()
		{
			using (var stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read))
			{
				if (stream.Length == 0)
					return;

				var serializer = new DataContractJsonSerializer(typeof(Value));
				setting = (Value)serializer.ReadObject(stream);
			}
		}

		static void Save()
		{
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Write))
			{
				var serializer = new DataContractJsonSerializer(typeof(Value));
				serializer.WriteObject(stream, setting);
				stream.Flush();
			}
		}

		public static string Name
		{
			get { return setting.name; }
			set
			{
				setting.name = value;
				Save();
			}
		}

		public static AccessToken AccessToken
		{
			get { return setting.accessToken; }
			set
			{
				setting.accessToken = value;
				Save();
			}
		}

		public static Uri ProcessUri
		{
			get { return setting.processUri; }
			set
			{
				setting.processUri = value;
				Save();
			}
		}

		public static bool IsAutomatized
		{
			get { return setting.isAutomatized; }
			set
			{
				setting.isAutomatized = value;
				Save();
			}
		}

		public static bool IsMaximized
		{
			get { return setting.isMaximized; }
			set
			{
				setting.isMaximized = value;
				Save();
			}
		}

		public static bool IsForefront
		{
			get { return setting.isForefronted; }
			set
			{
				setting.isForefronted = value;
				Save();
			}
		}
	}
}
