using Avalonia.Controls;
using Harmonica.Music;
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
		private string _buffering = "Not Initialized";
		public string Buffering
		{
			get => _buffering;
			private set => this.RaiseAndSetIfChanged(ref _buffering, value);
		}

		public MainWindowViewModel()
		{
			MusicManager.MusicPlayer.mediaPlayer.Buffering += MediaPlayer_Buffering;
		}

		private void MediaPlayer_Buffering(object? sender, MediaPlayerBufferingEventArgs e)
		{
			Buffering = $"Buffering: {e.Cache}%";
		}

		void PlayStop()
		{
			if (MusicManager.MusicPlayer.mediaPlayer.IsPlaying)
				MusicManager.MusicPlayer.Pause();
			else
				MusicManager.MusicPlayer.Unpause();
		}
	}
}
