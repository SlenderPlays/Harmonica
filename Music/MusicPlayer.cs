using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Harmonica.Music
{
	class MusicPlayer
	{
		public MediaPlayer mediaPlayer;

		
		public float buffer = 0;
		// This might need an added delay of 1 second or so, if network conditions are bad.
		public bool CanStartPlaying => buffer == 100;

		public MusicPlayer(LibVLC libVLC)
		{
			mediaPlayer = new MediaPlayer(libVLC);
			mediaPlayer.Buffering += MediaPlayer_Buffering;
		}

		public void Play(Media media)
		{
			media.AddOption(":no-video");
			ThreadPool.QueueUserWorkItem( async _ => {
				await media.Parse();
				mediaPlayer.Media = media;
				mediaPlayer.Play();
			});
		}
		public void Play(Media media, Action callback)
		{
			media.AddOption(":no-video");
			ThreadPool.QueueUserWorkItem(async _ => {
				await media.Parse();
				mediaPlayer.Media = media;
				mediaPlayer.Play();
				callback();
			});
		}

		public void PreparePlay(Media media)
		{
			media.AddOption(":no-video");
			ThreadPool.QueueUserWorkItem(async _ => {
				await media.Parse();
				mediaPlayer.Media = media;
				mediaPlayer.Play();
				Pause();				
			});
		}

		public void Pause() => mediaPlayer.SetPause(true);
		public void Unpause()
		{
			if (mediaPlayer.State == VLCState.Ended && mediaPlayer.Media != null)
			{
				Play(mediaPlayer.Media);
			}
			else
			{
				mediaPlayer.SetPause(false);
			}
		}

		public void SetVolume(int volume) => mediaPlayer.Volume = volume;

		public void Seek(long seconds, long minutes = 0, long hours = 0)
		{
			TimeSpan time =
				TimeSpan.FromHours(hours) +
				TimeSpan.FromMinutes(minutes) +
				TimeSpan.FromSeconds(seconds);
			mediaPlayer.SeekTo(time);
		}

		public void SeekForwards (long seconds) => Seek(mediaPlayer.Time / 1000 + seconds);
		public void SeekBackwards(long seconds) => Seek(mediaPlayer.Time / 1000 - seconds);
		// TODO: Next, Back, Shuffle, Repeat

		private void MediaPlayer_Buffering(object? sender, MediaPlayerBufferingEventArgs e) => buffer = e.Cache;

	}
}
