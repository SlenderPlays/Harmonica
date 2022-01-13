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
		private DynamicSlider? timeBar;
		private Label? currentTimeLabel;
		private Label? totalTimeLabel;
		private Button? playPauseButton;

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

			this.playPauseButton = this.FindControl<Button>("PlayPauseButton");
			MusicManager.MusicPlayer.mediaPlayer.EndReached += (s, e) => {
				// TODO: if repeat
				SetPlayState(false); 
			};


			this.currentTimeLabel = this.FindControl<Label>("CurrentTime");
			this.totalTimeLabel = this.FindControl<Label>("TotalTime");

			this.timeBar = this.FindControl<DynamicSlider>("TimeBar");
			this.timeBar.OnDragEnded += TimeBar_OnValueChanged;

			new Thread(async _ =>
			{
				while (Thread.CurrentThread.IsAlive)
				{
					Func<Task> task = () =>
					{
						if (!this.timeBar.IsDragging && MusicManager.MusicPlayer.mediaPlayer.Length > 0)
						{
							SetProgress(MusicManager.MusicPlayer.mediaPlayer.Length,
										MusicManager.MusicPlayer.mediaPlayer.Time);
						}
						return Task.CompletedTask;
					};

					await Dispatcher.UIThread.InvokeAsync(task);

					Thread.Sleep(100);
				}
			}).Start();
		}

		private void TimeBar_OnValueChanged(object? sender, double progressPercentage)
		{
			var newTime = progressPercentage * (MusicManager.MusicPlayer.mediaPlayer.Length / 1000);

			if (MusicManager.MusicPlayer.mediaPlayer.State == VLCState.Ended)
			{
				if (MusicManager.MusicPlayer.mediaPlayer.Media != null)
				{
					MusicManager.MusicPlayer.Play(MusicManager.MusicPlayer.mediaPlayer.Media, () => MusicManager.MusicPlayer.Seek((long)newTime));
				}
			}
			else
			{
				MusicManager.MusicPlayer.Seek((long)newTime);
			}
		}

		#region Buttons

		private void ReplaceClass(IControl target, string originalClass, string newClass)
		{
			if (target.Classes.Any(x => x == originalClass))
			{
				target.Classes.Remove(originalClass);
				target.Classes.Add(newClass);
			}
		}

		private bool TryReplaceClass(IControl target, string originalClass, string newClass)
		{
			bool anyMatch = false;
			if (target.Classes.Any(x => x == originalClass))
			{
				target.Classes.Remove(originalClass);
				target.Classes.Add(newClass);

				anyMatch = true;
			}

			return anyMatch;
		}

		public void TogglePlayState()
		{
			if (this.playPauseButton == null) return;

			if (this.playPauseButton.Classes.Contains("PlayButton"))
			{
				ReplaceClass(this.playPauseButton, "PlayButton", "PauseButton");
				MusicManager.MusicPlayer.Unpause();
			}
			else if(this.playPauseButton.Classes.Contains("PauseButton"))
			{
				ReplaceClass(this.playPauseButton, "PauseButton", "PlayButton");
				MusicManager.MusicPlayer.Pause();
			}
		}

		public void SetPlayState(bool play)
		{
			if (playPauseButton == null) return;

			if(play)
			{
				ReplaceClass(playPauseButton, "PlayButton", "PauseButton");
				MusicManager.MusicPlayer.Unpause();
			}
			else
			{
				ReplaceClass(playPauseButton, "PauseButton", "PlayButton");
				MusicManager.MusicPlayer.Pause();
			}
				
		}

		private void OnPlayButton_Pressed(object sender, RoutedEventArgs args)
		{
			var button = (Button)sender;
			if (TryReplaceClass(button, "PlayButton", "PauseButton"))
			{
				MusicManager.MusicPlayer.Unpause();
			}
			else if (TryReplaceClass(button, "PauseButton", "PlayButton"))
			{
				MusicManager.MusicPlayer.Pause();
			}
		}


		public void OnShuffleButton_Pressed(object sender, RoutedEventArgs args)
		{
			var button = (Button)sender;
			if (TryReplaceClass(button, "ShuffleOffButton", "ShuffleOnButton"))
			{
				// TODO: start shuffle
			}
			else if (TryReplaceClass(button, "ShuffleOnButton", "ShuffleOffButton"))
			{
				// stop shuffle
			}
		}

		public void OnRepeatButton_Pressed(object sender, RoutedEventArgs args)
		{
			var button = (Button)sender;
			if (TryReplaceClass(button, "RepeatOffButton", "RepeatOnButton"))
			{
				// TODO: repeat (all)
			}
			else if (TryReplaceClass(button, "RepeatOnButton", "RepeatOneButton"))
			{
				// repeat (one/same)
			}
			else if (TryReplaceClass(button, "RepeatOneButton", "RepeatOffButton"))
			{
				// stop repeat
			}
		}

		public void OnPreviousButton_Pressed(object sender, RoutedEventArgs args)
		{
			// TODO: previous
		}

		public void OnNextButton_Pressed(object sender, RoutedEventArgs args)
		{
			// TODO: next
		}

		#endregion
		#region ProgressBar

		public void SetProgress(long totalTime, long currentTime)
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

		#endregion
	}
}
