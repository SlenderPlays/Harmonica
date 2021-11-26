using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Harmonica.Music;
using LibVLCSharp.Shared;
using System;
using System.Linq;

namespace Harmonica.Views
{
	public partial class MainWindow : Window
	{
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
		}

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
	}
}
