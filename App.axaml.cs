using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Harmonica.Music;
using Harmonica.ViewModels;
using Harmonica.Views;
using LibVLCSharp.Shared;

namespace Harmonica
{
	public class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
			if (!Design.IsDesignMode)
				MusicManager.MusicPlayer.PreparePlay(
					 MusicManager.MediaLocator.GetMedia(MusicManager.LibVLC, "pony_island_soundtrack_mp3/02 - Enter Pony.mp3")
				);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow
				{
					DataContext = new MainWindowViewModel(),
				};
			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}
