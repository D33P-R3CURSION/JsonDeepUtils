using Newtonsoft.Json;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SeamSearchLaserScan.Logic.ProjectSettings
{
	public sealed class ProjectSettingsLoader<T>
	{
		#region Constructors
		public ProjectSettingsLoader(SettingsData projectSettings)
		{
			_projectSettingsData = projectSettings;

			if (File.Exists(_jsonFilePath) == false)
			{
                serialization(_jsonFilePath, _settingsDir);
            }
			else
			{
				deserialization();
			}
		}
		#endregion Constructors

		#region Properties
		public SettingsData Settings
		{
			get
			{
				lock (_settingsLock)
				{
					return _projectSettingsData;
				}
			}
		}
		#endregion Properties

		#region Methods
		public void WriteSettings()
		{
			serialization(_jsonFilePath, _settingsDir);
		}

		public void LoadSettings()
		{
			deserialization();
		}

		private void rewriteProperties<T>(T oldData, T newData)
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
				if (property.PropertyType.BaseType == typeof(SettingsData))
				{
					rewriteProperties(property.GetValue(oldData), property.GetValue(newData));
				}
				else
				{
					property.SetValue(oldData, property.GetValue(newData));
				}
			}
		}

		private void serialization(string fullFilePath, string dir)
		{
			string jsonString = JsonConvert.SerializeObject(_projectSettingsData, Formatting.Indented);

			if (Directory.Exists(dir) == false)
			{
				Directory.CreateDirectory(dir);
			}

			File.WriteAllText(fullFilePath, jsonString);
		}

		private void deserialization()
		{
			string jsonString = File.ReadAllText(_jsonFilePath);

			try
			{
                T? newData = JsonConvert.DeserializeObject<T>(jsonString);
				if (newData != null && newData is SettingsData settingsData)
				{
					rewriteProperties(_projectSettingsData, settingsData);
					serialization(_jsonFilePath, _settingsDir);
				}
			}
			catch
			{
				//logger
			}
		}
		#endregion

		#region Fields
		private readonly object _settingsLock = new object();

		private const string _settingsDir = "Settings";
		private const string _jsonFileName = "ProjectSettings.json";
		private string _jsonFilePath => Path.Combine(_settingsDir, _jsonFileName);
		private SettingsData _projectSettingsData;
		#endregion
	}
}
