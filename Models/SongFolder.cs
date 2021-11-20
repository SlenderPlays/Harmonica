using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Harmonica.Models
{
	class SongFolder
	{
		public SongFolder(string path)
		{
			this.FolderPath = path;

			string name = new DirectoryInfo(path).Name;
			this.WithName(name);
		}

		public const string DefaultName = "Unnamed Folder";

		public string FolderPath { get; private set; }
		public string Name { get; set; } = DefaultName;
		public List<Song> Songs { get; set; } = new List<Song>();
		public List<SongFolder> SongFolders { get; set; } = new List<SongFolder>();

		public SongFolder WithName(string? name)
		{
			this.Name = String.IsNullOrWhiteSpace(name) ? DefaultName : name;
			return this;
		}
		public SongFolder WithSongs(List<Song> songs)
		{
			this.Songs = songs ?? new List<Song>();
			return this;
		}
		public SongFolder WithSongFolders(List<SongFolder> songFolders)
		{
			this.SongFolders = songFolders ?? new List<SongFolder>();
			return this;
		}

	}
}
