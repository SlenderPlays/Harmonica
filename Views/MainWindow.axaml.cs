using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Harmonica.Controls;
using Harmonica.Music;
using LibVLCSharp.Shared;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Harmonica.Views
{
	public partial class MainWindow : Window
	{
		#nullable disable
		// Player Controls is never going to be null after the constructor, but IntelliSense doesn't recognize helper methods
		private PlayerControls playerControls;
		#nullable enable

		private MusicPlayer musicPlayer => MusicManager.MusicPlayer;

		private DynamicSlider? timeBar;
		private Label? currentTimeLabel;
		private Label? totalTimeLabel;

		public MainWindow()
		{
			InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
			
			// Initialize Player Controls
			Button playPauseButton = this.FindControl<Button>("PlayPauseButton");
			Button shuffleButton = this.FindControl<Button>("ShuffleButton");
			Button repeatButton = this.FindControl<Button>("RepeatButton");
			playerControls = new PlayerControls(playPauseButton, shuffleButton, repeatButton);

			// Handle Media Player End
			musicPlayer.mediaPlayer.EndReached += MusicPlayer_EndReached;


			this.currentTimeLabel = this.FindControl<Label>("CurrentTime");
			this.totalTimeLabel = this.FindControl<Label>("TotalTime");

			this.timeBar = this.FindControl<DynamicSlider>("TimeBar");
			this.timeBar.OnDragEnded += TimeBar_OnTimeSet;

			this.FindControl<DynamicSlider>("VolumeSlider").OnDragEnded += VolumeSlider_OnVolumeSet;

			new Thread(async _ =>
			{
				while (Thread.CurrentThread.IsAlive)
				{
					await Dispatcher.UIThread.InvokeAsync(new Action( () => {
						if (!this.timeBar.IsDragging && musicPlayer.mediaPlayer.Length > 0)
						{
							UpdateTimeBarText(musicPlayer.mediaPlayer.Length,
										musicPlayer.mediaPlayer.Time);
						}
					}));

					Thread.Sleep(100);
				}
			}).Start();
		}

		private void VolumeSlider_OnVolumeSet(object? sender, double e)
		{
			musicPlayer.SetVolume((int)e);
		}

		private void MusicPlayer_EndReached(object? sender, EventArgs e)
		{
			if (playerControls.CurrentRepeatSate == RepeatState.REPEAT_ON)
			{
				// TODO: Repeat Queue
			}
			else if (playerControls.CurrentRepeatSate == RepeatState.REPEAT_ONE)
			{
				// Repeat same song
				musicPlayer.Unpause();
			}
			else
			{
				// TODO: next in queue
				// if queue is empty
				playerControls.SetPlayState(PlayState.STOPPED);
			}
		}

		#region Buttons

		private void OnPlayButton_Pressed(object sender, RoutedEventArgs args)
		{
			playerControls.TogglePlayState();

			if(playerControls.CurrentPlayState == PlayState.PLAYING)
			{
				musicPlayer.Unpause();
			}
			else
			{
				musicPlayer.Pause();
			}

		}


		public void OnShuffleButton_Pressed(object sender, RoutedEventArgs args)
		{
			playerControls.ToggleShuffleState();

			if (playerControls.CurrentShuffleState == ShuffleState.SHUFFLE_ON)
			{
				// TODO: Enable Shuffle
			}
			else
			{
				// TODO: Disable Shuffle
			}

			
		}

		public void OnRepeatButton_Pressed(object sender, RoutedEventArgs args)
		{
			playerControls.ToggleRepeatState();

			if(playerControls.CurrentRepeatSate == RepeatState.REPEAT_ON)
			{
				// TODO: Toggle queue repeat ON
			}
			else if(playerControls.CurrentRepeatSate == RepeatState.REPEAT_OFF || playerControls.CurrentRepeatSate == RepeatState.REPEAT_ONE)
			{
				// TODO: Toggle queue repeat OFF
			}
		}

		public void OnPreviousButton_Pressed(object sender, RoutedEventArgs args)
		{
			// TODO: previous in queue
		}

		public void OnNextButton_Pressed(object sender, RoutedEventArgs args)
		{
			// TODO: next in queue
		}

		#endregion
		#region TimeBar

		public void UpdateTimeBarText(long totalTime, long currentTime)
		{
			if (totalTime == -1) totalTime = 0;
			if (currentTime == -1) currentTime= 0;

			// TODO: label changes size when text changes... probably needs padding or smth
			// TODO: make corner radius of progress bar only on the corners, and not on the inside

			// TODO: make progress bar actually a slider
			string totalString = TimeSpan.FromMilliseconds(totalTime).ToString("mm\\:ss");
			string currentString = TimeSpan.FromMilliseconds(currentTime).ToString("mm\\:ss");

			double progress =  totalTime == 0 ? 0: (double)currentTime / totalTime;
			
			if (timeBar != null) timeBar.Value = progress;
			if (totalTimeLabel != null) totalTimeLabel.Content = totalString;
			if (currentTimeLabel != null) currentTimeLabel.Content = currentString;
		}

		private void TimeBar_OnTimeSet(object? sender, double progressPercentage)
		{
			var newTime = progressPercentage * (musicPlayer.mediaPlayer.Length / 1000);

			if (musicPlayer.mediaPlayer.State == VLCState.Ended)
			{
				if (musicPlayer.mediaPlayer.Media != null)
				{
					musicPlayer.Play(musicPlayer.mediaPlayer.Media, () => musicPlayer.Seek((long)newTime));
				}
			}
			else
			{
				musicPlayer.Seek((long)newTime);
			}

			playerControls.SetPlayState(PlayState.PLAYING);
		}

		#endregion
	}
}
