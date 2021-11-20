using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonica.Music
{
	class MusicManager
	{

		private static Lazy<MusicManager> _instance = new Lazy<MusicManager>(() => new MusicManager());
		public static MusicManager Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		// TODO: private?
		public LibVLC libVLC;
		public MusicPlayer MusicPlayer { get; private set; }
		private MediaLocator mediaLocator;

		public string MusicPath { get; private set; }

		private MusicManager()
		{
			LibVLCSharp.Shared.Core.Initialize();

			libVLC = new LibVLC();
			MusicPlayer = new MusicPlayer(libVLC);
			// TODO: Get path from config...
			MusicPath = "D:/Programming/Atestat/WebServer/songs/";
			mediaLocator = new MediaLocator(MusicPath);
		}
	}
}
