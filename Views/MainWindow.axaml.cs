using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Harmonica.Controls;
using Harmonica.Models;
using Harmonica.Music;
using LibVLCSharp.Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
		private MediaLocator mediaLocator => MusicManager.MediaLocator;

		private DynamicSlider? timeBar;
		private Label? currentTimeLabel;
		private Label? totalTimeLabel;

		private Border? QueueHolder;
		private StackPanel? QueueList;
		private Separator? QueueSeparator;
		private Label? EOQLabel;
		
		private StackPanel? SongExplorer;

		private Image? BackgroundImage;
		private Label? BackgroundTitle;
		private Label? BackgroundAuthors;

		private bool changingSong = false;

		private Label? SelectedFolderLabel;
		private Button? SelectedFolderButton;

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

			// Handle Time Bar
			this.currentTimeLabel = this.FindControl<Label>("CurrentTime");
			this.totalTimeLabel = this.FindControl<Label>("TotalTime");

			this.timeBar = this.FindControl<DynamicSlider>("TimeBar");
			this.timeBar.OnDragEnded += TimeBar_OnTimeSet;

			DynamicSlider volumeSlider = this.FindControl<DynamicSlider>("VolumeSlider");
			volumeSlider.Value = SaveManager.settings.volume;
			VolumeSlider_OnVolumeSet(this, volumeSlider.Value);
			volumeSlider.OnDragEnded += VolumeSlider_OnVolumeSet;

			new Thread(async _ =>
			{
				while (Thread.CurrentThread.IsAlive)
				{
					await Dispatcher.UIThread.InvokeAsync(new Action(() =>
					{
						if (!this.timeBar.IsDragging && musicPlayer.mediaPlayer.Length > 0)
						{
							UpdateTimeBarText(musicPlayer.mediaPlayer.Length,
										musicPlayer.mediaPlayer.Time);
						}
					}));

					Thread.Sleep(100);
				}
			}).Start();

			// Populate

			QueueHolder = this.FindControl<Border>("QueueHolder");
			QueueList = this.FindControl<StackPanel>("QueueList");

			QueueSeparator = this.FindControl<Separator>("Separator");
			EOQLabel = this.FindControl<Label>("EOQLabel");

			SongExplorer = this.FindControl<StackPanel>("SongExplorer");

			PopulateSongExplorer();
			PopulateQueue();
			mediaLocator.rootFolderChanged += () =>
			{
				Dispatcher.UIThread.InvokeAsync(PopulateSongExplorer);
				Dispatcher.UIThread.InvokeAsync(UpdateSelectedFolder);
			};
			MusicManager.Instance.SongQueue.QueueChanged += () =>
			{
				Dispatcher.UIThread.InvokeAsync(PopulateQueue);
				if(!changingSong && MusicManager.Instance.SongQueue.Size == 1 && (musicPlayer.mediaPlayer.Media == null || musicPlayer.mediaPlayer.Media.State == VLCState.Ended))
				{
					Song s = GetNextFromQueue();
					musicPlayer.Play(mediaLocator.GetMediaAbsolute(MusicManager.LibVLC, s.FilePath));
				}
			};

			BackgroundAuthors = this.FindControl<Label>("BackgroundAuthors");
			BackgroundTitle = this.FindControl<Label>("BackgroundTitle");
			BackgroundImage = this.FindControl<Image>("BackgroundImage");

			musicPlayer.SongDirectlyPlayed += (sender, song ) => Dispatcher.UIThread.InvokeAsync(() => UpdateBagkround(song));

			SelectedFolderLabel = this.FindControl<Label>("SelectedFolderLabel");
			SelectedFolderButton = this.FindControl<Button>("SelectFolderButton");
			SelectedFolderButton.Click += async (s, e) => {
				OpenFolderDialog dialog = new OpenFolderDialog();
				string? path = await dialog.ShowAsync(this).ConfigureAwait(false);
				if (path != null)
				{
					SaveManager.settings.songFolder = path;
					mediaLocator.Initialize(path);
					Dispatcher.UIThread.InvokeAsync(() => UpdateSelectedFolder());
				}

			};
			
			UpdateSelectedFolder();
		}

		public void UpdateSelectedFolder()
		{
			if(SelectedFolderLabel != null && SelectedFolderButton != null)
			{
				if (String.IsNullOrWhiteSpace(mediaLocator.musicPath))
				{
					SelectedFolderLabel.Content = "No folder selected";
					SelectedFolderButton.Content = "Select";
				}
				else
				{
					SelectedFolderLabel.Content = mediaLocator.musicPath;
					SelectedFolderButton.Content = "Change";
				}
			}
		}

		public void PopulateQueue()
		{
			if (QueueList != null)
			{
				QueueList.Children.Clear();
				foreach (Song song in MusicManager.Instance.SongQueue.ToList())
				{
					QueuedSong newControl = new QueuedSong(song);
					QueueList.Children.Add(newControl);
				}
			}

			if (EOQLabel != null && QueueSeparator != null)
			{
				if (MusicManager.Instance.SongQueue.Empty())
				{
					EOQLabel.Content = "Queue Is Empty";
					QueueSeparator.IsVisible = false;
				}
				else
				{
					EOQLabel.Content = "End Of Queue";
					QueueSeparator.IsVisible = true;
				}
			}
		}

		public void PopulateSongExplorer()
		{
			if (SongExplorer != null)
			{
				SongExplorer.Children.Clear();
				PopulateSongFolder(SongExplorer, mediaLocator.rootFolder);
			}
		}

		private void PopulateSongFolder(StackPanel parentControl, SongFolder rootFolder)
		{
			if (rootFolder == null) return;

			foreach(SongFolder songFolder in rootFolder.SongFolders)
			{
				SongFolderControl songFolderControl = new SongFolderControl();
				songFolderControl.Title = songFolder.Name;
				songFolderControl.Expanded = false;

				StackPanel songFolderContent = new StackPanel();
				songFolderControl.Content = songFolderContent;

				parentControl.Children.Add(songFolderControl);
				PopulateSongFolder(songFolderContent, songFolder);
			}

			foreach(Song song in rootFolder.Songs)
			{
				SongControl songControl = new SongControl(song);

				parentControl.Children.Add(songControl);
			}
		}

		private void VolumeSlider_OnVolumeSet(object? sender, double e)
		{
			musicPlayer.SetVolume((int)e);
			SaveManager.settings.volume = (int)e;
		}

		private void MusicPlayer_EndReached(object? sender, EventArgs e)
		{
			changingSong = true;
			if (playerControls.CurrentRepeatSate == RepeatState.REPEAT_ON)
			{
				// Repeat Queue
				Song s = GetNextFromQueue();

				MusicManager.Instance.SongQueue.Enqueue(s);

				musicPlayer.Play(mediaLocator.GetMediaAbsolute(MusicManager.LibVLC, s.FilePath));

			}
			else if (playerControls.CurrentRepeatSate == RepeatState.REPEAT_ONE)
			{
				// Repeat same song
				musicPlayer.Unpause();
			}
			else
			{
				// Next in Queue
				if (MusicManager.Instance.SongQueue.Empty())
				{
					playerControls.SetPlayState(PlayState.STOPPED);
				}
				else
				{
					Song s = GetNextFromQueue();
					musicPlayer.Play(mediaLocator.GetMediaAbsolute(MusicManager.LibVLC, s.FilePath));
				}
			}
			changingSong = false;
		}

		private Song GetNextFromQueue()
		{
			Song s;
			if (playerControls.CurrentShuffleState == ShuffleState.SHUFFLE_OFF)
			{
				s = MusicManager.Instance.SongQueue.Dequeue();
			}
			else
			{
				s = MusicManager.Instance.SongQueue.DequeueRandom();
			}

			Dispatcher.UIThread.InvokeAsync(() => UpdateBagkround(s));

			return s;
		}

		private void UpdateBagkround(Song s)
		{
			// TODO: Update background if user hits play, instead of queue
			if (BackgroundImage != null)
				BackgroundImage.Source = s.Thumbnail;
			if (BackgroundTitle != null)
				BackgroundTitle.Content = s.Title;
			if (BackgroundAuthors != null)
				BackgroundAuthors.Content = String.Join(", ", s.Authors);
		}

		#region Buttons

		private void OnPlayButton_Pressed(object sender, RoutedEventArgs args)
		{
			// TODO: get next item from queue if item is done playing, and the queue is not empty
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
		}

		public void OnRepeatButton_Pressed(object sender, RoutedEventArgs args)
		{
			playerControls.ToggleRepeatState();
		}

		public void OnPreviousButton_Pressed(object sender, RoutedEventArgs args)
		{
			// Can't be arsed to make a "previous" queue, so you get to play this song back from the start.
			musicPlayer.Seek(0);
		}

		public void OnNextButton_Pressed(object sender, RoutedEventArgs args)
		{
			musicPlayer.Unpause();
			musicPlayer.EndTrack();
		}

		public void OnQueueButton_Pressed(object sender, RoutedEventArgs args)
		{
			if (QueueHolder == null) return;

			QueueHolder.IsVisible = !QueueHolder.IsVisible;
		}

		#endregion
		#region TimeBar

		public void UpdateTimeBarText(long totalTime, long currentTime)
		{
			if (totalTime == -1) totalTime = 0;
			if (currentTime == -1) currentTime= 0;

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

		protected override void OnClosing(CancelEventArgs e)
		{
			SaveManager.Save();
			base.OnClosing(e);
		}

		#endregion
	}
}
