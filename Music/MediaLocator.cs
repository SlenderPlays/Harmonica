using System.IO;

namespace Harmonica.Music
{
	class MediaLocator
	{
		private string musicPath;
		private FileSystemWatcher watcher;


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
			watcher.Changed += FSW_Event;

			ScanMusicFolder();
		}

		public void ScanMusicFolder()
		{
			// Will create the songs, playlists. The MusicPlayer will have REFERENCES to songs (hash in multi, file/song name on single)
			// that can be de-amiguated here? or somewhere else?
		}

		private void FSW_Event(object sender, FileSystemEventArgs e) => ScanMusicFolder();
	}
}