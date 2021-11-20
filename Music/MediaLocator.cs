using Harmonica.Models;
using System.Collections.Generic;
using System.IO;

namespace Harmonica.Music
{
	class MediaLocator
	{
		private string musicPath;
		private FileSystemWatcher watcher;

		public SongFolder rootFolder;

		public MediaLocator(string musicPath)
		{
			this.musicPath = musicPath;

			watcher = new FileSystemWatcher(musicPath, "*.*");
			watcher.IncludeSubdirectories = true;
			// On/Off switch
			watcher.EnableRaisingEvents = true;

			watcher.NotifyFilter =
				NotifyFilters.LastWrite
				| NotifyFilters.FileName
				| NotifyFilters.DirectoryName
				| NotifyFilters.Attributes;

			watcher.Changed += FSW_Event;
			watcher.Created += FSW_Event;
			watcher.Deleted += FSW_Event;
			watcher.Renamed += FSW_Event;

			rootFolder = ScanMusicFolder(musicPath);
		}

		public SongFolder ScanMusicFolder(string path)
		{
			SongFolder songFolder = new SongFolder(path);

			foreach (string filePath in Directory.GetFiles(path))
			{
				Song song = new Song(filePath);
					
				songFolder.Songs.Add(song);
			}
			foreach (string folderPath in Directory.GetDirectories(path))
			{
				songFolder.SongFolders.Add(ScanMusicFolder(folderPath));
			}

			return songFolder;
		}

		private void FSW_Event(object sender, FileSystemEventArgs e) {
			// TODO: what to do with the old music, especially if it is currently playing, or if a playlist already exists?
			// IDEAS:
			//// Will create the songs, playlists. The MusicPlayer will have REFERENCES to songs (hash in multi, file/song name on single)
			////  that can be de-amiguated here? or somewhere else?

			rootFolder = ScanMusicFolder(musicPath);
		}
	}
}