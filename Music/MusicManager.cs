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

		public static LibVLC LibVLC {
			get => Instance.libVLC;
			private set => Instance.libVLC = value;
		}
		public static MusicPlayer MusicPlayer
		{
			get => Instance.musicPlayer;
			private set => Instance.musicPlayer = value;
		}
		public static MediaLocator MediaLocator
		{
			get => Instance.mediaLocator;
			private set => Instance.mediaLocator = value;
		}

		private LibVLC libVLC;
		private MusicPlayer musicPlayer;
		private MediaLocator mediaLocator;

		public string MusicPath { get; private set; }

		private MusicManager()
		{
			LibVLCSharp.Shared.Core.Initialize();

			libVLC = new LibVLC();
			musicPlayer = new MusicPlayer(libVLC);
			// TODO: Get path from config... Really need to do this soon
			MusicPath = "C:/PCFiles/Programming/Atestat/WebServer/songs/";
			mediaLocator = new MediaLocator(MusicPath);
		}
	}
}
