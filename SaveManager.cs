using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Harmonica
{
	public static class SaveManager
	{
		public static string config_file = "";
		public static Settings settings = new Settings(); 

		public static void EnsureExistence()
		{
			string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			if (!appdata.EndsWith('\\') && !appdata.EndsWith('/'))
			{
				appdata += "/";
			}
			string config_folder = appdata + "Harmonica";
			if(!Directory.Exists(config_folder))
			{
				Directory.CreateDirectory(config_folder);
			}

			if (!config_folder.EndsWith('\\') && !config_folder.EndsWith('/'))
			{
				config_folder += "/";
			}
			config_file = config_folder + "user_config.dat";

			if(!File.Exists(config_file))
			{
				File.WriteAllText(config_file, JsonSerializer.Serialize<Settings>(settings));
			}
		}

		public static void Save()
		{
			EnsureExistence();

			File.WriteAllText(config_file, JsonSerializer.Serialize(settings));
		}

		public static void Load()
		{
			EnsureExistence();

			var json = File.ReadAllText(config_file);

			settings = JsonSerializer.Deserialize<Settings>(json);
		}
	}

	public class Settings
	{
		public string? songFolder { get; set; } = null;
		public int volume { get; set; } = 50;
	}

}
