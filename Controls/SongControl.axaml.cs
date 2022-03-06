using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Harmonica.Models;
using Harmonica.Music;
using LibVLCSharp.Shared;
using System;

namespace Harmonica.Controls
{
	public class SongControl : TemplatedControl
	{
		public Song? LinkedSong { get; set; }

#nullable disable
		private Image Thumbnail;
		private Label TitleLable;
		private Label AuthorsLabel;
		private Label AlbumLabel;
		private Label DurationLevel;
		private Button PlayButton;
		private Button QueueButton;
#nullable enable

		public void UpdateUI()
		{
			if (LinkedSong == null) return;

			Thumbnail.Source = LinkedSong.Thumbnail;
			TitleLable.Content = LinkedSong.Title;
			AlbumLabel.Content = LinkedSong.Album;
			DurationLevel.Content = LinkedSong.Duration.ToString("mm\\:ss");
			AuthorsLabel.Content = "by " + String.Join(", ", LinkedSong.Authors);
		}

		public SongControl()
		{
		}

		public SongControl(Song song) : this()
		{
			LinkedSong = song;
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			Thumbnail = e.NameScope.Get<Image>("Thumbnail");
			TitleLable = e.NameScope.Get<Label>("TitleLabel");
			AuthorsLabel = e.NameScope.Get<Label>("AuthorsLabel");
			AlbumLabel = e.NameScope.Get<Label>("AlbumLabel");
			DurationLevel = e.NameScope.Get<Label>("DurationLabel");
			PlayButton = e.NameScope.Get<Button>("PlayButton");
			QueueButton = e.NameScope.Get<Button>("QueueButton");

			PlayButton.Click += PlayButton_Clicked;
			QueueButton.Click += QueueButton_Clicked;

			UpdateUI();
		}

		private void QueueButton_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void PlayButton_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (LinkedSong == null) return;

			MusicManager.MusicPlayer.Play(MusicManager.MediaLocator.GetMediaAbsolute(MusicManager.LibVLC, LinkedSong.FilePath));
		}
	}
}
