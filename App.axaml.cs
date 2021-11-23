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
					new Media(MusicManager.LibVLC,"https://archive.org/download/ImagineDragons_201410/imagine%20dragons.mp4", FromType.FromLocation)
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
