using Newtonsoft.Json;
using System.Reflection;

namespace JsonDeepUtils
{
	public sealed class JsonLoader<T>
	{
		#region Constructors
		public JsonLoader(IParseData projectSettings)
		{
			_data = projectSettings;

			if (!File.Exists(FilePath))
			{
				Serialization(FilePath, _dir);
			}
			else
			{
				Deserialization();
			}
		}
		#endregion Constructors

		#region Properties
		public T Settings
		{
			get
			{
				if (_data is T data)
				{
					lock (DataLock)
					{
						return data;
					}
				}
				else
				{
					throw new InvalidOperationException("Не соответсвие типов");
				}
			}
		}
		#endregion Properties

		#region Methods
		public void Write()
		{
			Serialization(FilePath, _dir);
		}

		public void Load()
		{
			Deserialization();
		}

		private void RewriteProperties<T>(T oldData, T newData)
		{
			if (newData == null)
			{
				return;
			}

			if (oldData == null)
			{
				oldData = newData;
				return;
			}

			PropertyInfo[] propertyes = newData.GetType().GetProperties();
			foreach (PropertyInfo property in propertyes)
			{
				if (property.PropertyType.BaseType == typeof(IParseData))
				{
					RewriteProperties(property.GetValue(oldData), property.GetValue(newData));
				}
				else
				{
					property.SetValue(oldData, property.GetValue(newData));
				}
			}
		}

		private void Serialization(string fullFilePath, string dir)
		{
			if (_data is not T projData)
			{
				return;
			}

			string jsonString = JsonConvert.SerializeObject(projData, Formatting.Indented);

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			File.WriteAllText(fullFilePath, jsonString);
		}

		private void Deserialization()
		{
			string jsonString = File.ReadAllText(FilePath);

			try
			{
				T? newData = JsonConvert.DeserializeObject<T>(jsonString);
				if (newData != null && newData is IParseData settingsData)
				{
					RewriteProperties(_data, settingsData);
					Serialization(FilePath, _dir);
				}
			}
			catch
			{
				//logger
			}
		}
		#endregion

		#region Fields
		private readonly Lock DataLock = new();

		private const string _dir = "Settings";
		private const string _jsonFileName = "ProjectSettings.json";
		private static string FilePath => Path.Combine(_dir, _jsonFileName);
		private readonly IParseData _data;
		#endregion
	}
}
