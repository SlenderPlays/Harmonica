using LibVLCSharp.Shared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
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

		LibVLC libVLC;
		MediaPlayer mp;


		public MainWindowViewModel()
		{
			LibVLCSharp.Shared.Core.Initialize();

			new Thread(async () =>
			{
			   libVLC = new LibVLC();

			   var media = new Media(libVLC, "file:///C:/Users/Geo/Downloads/redditsave.com_rare_arbys_commercial_original_shitpost_by-p8yeah30tck71.mp4", FromType.FromLocation, ":no-video");
			   await media.Parse(MediaParseOptions.ParseNetwork);

			   mp = new MediaPlayer(media);
			   mp.Play();


			   Greeting = "Initialized";
		   }).Start();
		}

		void PlayStop()
		{
			if (mp.IsPlaying) mp.Stop();
			else mp.Play();
		}
	}
}
