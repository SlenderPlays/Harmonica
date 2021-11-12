using Avalonia.Controls;
using LibVLCSharp.Shared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Harmonica.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private string _greeting = "Not Initialized";
		public string Greeting
		{
			get => _greeting;
			private set => this.RaiseAndSetIfChanged(ref _greeting, value);
		}

		private string _buffering = "Not Initialized";
		public string Buffering
		{
			get => _buffering;
			private set => this.RaiseAndSetIfChanged(ref _buffering, value);
		}

		private string _pausable = "Not Initialized";
		public string Pausable
		{
			get => _pausable;
			private set => this.RaiseAndSetIfChanged(ref _pausable, value);
		}

		LibVLC libVLC;
		MediaPlayer mp;


		public MainWindowViewModel()
		{
			if (Design.IsDesignMode) return;
			LibVLCSharp.Shared.Core.Initialize();

			new Thread(async () =>
			{
				string temp = "C:/Users/Geo/AppData/Local/Temp";
				libVLC = new LibVLC();

				// Downloading is slow...
				//using (var client = new WebClient())
				//{
				//	client.DownloadFile("https://archive.org/download/ImagineDragons_201410/imagine%20dragons.mp4", temp + "/dragons.mp4");
				//}


				// var media = new Media(libVLC, "file:///C:/Users/Geo/Downloads/redditsave.com_rare_arbys_commercial_original_shitpost_by-p8yeah30tck71.mp4", FromType.FromLocation, ":no-video");
				// var media = new Media(libVLC, "https://www.youtube.com/watch?v=dQw4w9WgXcQ", FromType.FromLocation, ":no-video"); // VLC needs updating?
				 var media = new Media(libVLC, "https://archive.org/download/ImagineDragons_201410/imagine%20dragons.mp4", FromType.FromLocation, ":no-video"); // Slow due to server slow
				// var media = new Media(libVLC, $"file://{temp}/dragons.mp4", FromType.FromLocation, ":no-video"); // Very slow do download, 50 MB
				// var media = new Media(libVLC, $"http://192.168.0.139:8080/dragons.mp4", FromType.FromLocation, ":no-video");
				
				await media.Parse(MediaParseOptions.ParseNetwork | MediaParseOptions.FetchNetwork);
				// await media.SubItems.First().Parse(MediaParseOptions.ParseNetwork); // For youtube

				// mp = new MediaPlayer(media.SubItems.First()); // For youtbe
				mp = new MediaPlayer(media);
				mp.Buffering += Mp_Buffering;
				mp.PausableChanged += Mp_PausableChanged;

				mp.Play();
				mp.SetPause(true); // mp.Pause(); doesn't work... weird

				// Can't really determine when its done buffering, best thing to do is to download the file, and let the media player buffer from downloaded file.
				// new Media( libVLC,new StreamMediaInput(STREAM)) // maybe I can do some stream magickery, otherwise I'm probably going to need to wrangle with
				// the buffer myself to get it to work like I want. 
				Greeting = "Initialized";
		   }).Start();
		}

		private void Mp_PausableChanged(object? sender, MediaPlayerPausableChangedEventArgs e)
		{
			Pausable = $"Pausable: {e.Pausable}";
		}

		private void Mp_Buffering(object? sender, MediaPlayerBufferingEventArgs e)
		{
			Buffering = $"Buffering {e.Cache}";
		}

		void PlayStop()
		{
			if (mp.IsPlaying) mp.Pause();
			else mp.Play();
		}
	}
}
