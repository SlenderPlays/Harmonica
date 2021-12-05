using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
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
		private ProgressBar? timeBar;
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

			currentTimeLabel = this.FindControl<Label>("CurrentTime");
			totalTimeLabel = this.FindControl<Label>("TotalTime");

			timeBar = this.FindControl<ProgressBar>("TimeBar");
			//timeBar.RenderTransform = new RotateTransform(180);
			new Thread(async _ =>
			{
				while (Thread.CurrentThread.IsAlive)
				{
					Func<Task> task = () =>
					{
						SetProgress(MusicManager.MusicPlayer.mediaPlayer.Length, 
									MusicManager.MusicPlayer.mediaPlayer.Time);
						return Task.CompletedTask;
					};

					await Dispatcher.UIThread.InvokeAsync(task);

					Thread.Sleep(100);
				}
			}).Start();
		}

		#region PlayerControls
		#region Buttons

		private bool TryReplaceClass(IControl target, string originalClass, string newClass)
		{
			if (target.Classes.Any(x => x == originalClass))
			{
				target.Classes.Remove(originalClass);
				target.Classes.Add(newClass);
				return true;
			}
			return false;
		}

		public void OnPlayButton_Pressed(object sender, RoutedEventArgs args)
		{
			var button = (Button)sender;
			if (TryReplaceClass(button, "PlayButton", "PauseButton"))
			{
				MusicManager.MusicPlayer.Unpause();
			}
			else if(TryReplaceClass(button, "PauseButton", "PlayButton"))
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
			
			if (timeBar != null) timeBar.Value = 1 - progress;
			if (totalTimeLabel != null) totalTimeLabel.Content = totalString;
			if (currentTimeLabel != null) currentTimeLabel.Content = currentString;
		}

		#endregion
		#endregion
	}
}
