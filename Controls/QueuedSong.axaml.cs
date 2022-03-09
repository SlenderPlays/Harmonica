using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Harmonica.Models;
using Harmonica.Music;
using System;

namespace Harmonica.Controls
{
	public class QueuedSong : TemplatedControl
	{
		public Song? LinkedSong { get; set; }

#nullable disable
		private Image Thumbnail;
		private Label TitleLable;
		private Label AuthorsLabel;
		private Label DurationLevel;
		private Button UpButton;
		private Button DownButton;
#nullable enable

		public void UpdateUI()
		{
			if (LinkedSong == null) return;

			Thumbnail.Source = LinkedSong.Thumbnail;
			TitleLable.Content = LinkedSong.Title;			
			DurationLevel.Content = LinkedSong.Duration.ToString("mm\\:ss");
			AuthorsLabel.Content = "by " + String.Join(", ", LinkedSong.Authors);
		}

		public QueuedSong()
		{
		}

		public QueuedSong(Song song) : this()
		{
			LinkedSong = song;
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			Thumbnail = e.NameScope.Get<Image>("Thumbnail");
			TitleLable = e.NameScope.Get<Label>("TitleLabel");
			AuthorsLabel = e.NameScope.Get<Label>("AuthorsLabel");
			DurationLevel = e.NameScope.Get<Label>("DurationLabel");
			UpButton = e.NameScope.Get<Button>("UpButton");
			DownButton = e.NameScope.Get<Button>("DownButton");

			UpButton.Click += UpButton_Clicked;
			DownButton.Click += DownButton_Clicked;

			UpdateUI();
		}

		private void DownButton_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (LinkedSong == null) return;

			MusicManager.Instance.SongQueue.MoveDown(LinkedSong);
		}

		private void UpButton_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (LinkedSong == null) return;

			MusicManager.Instance.SongQueue.MoveUp(LinkedSong);
		}
	}
}
