using Avalonia.Controls;
using Avalonia.Interactivity;
using Harmonica.Models;
using Harmonica.Music;
using LibVLCSharp.Shared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Harmonica.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public SongFolder RootFolder
		{
			get => MusicManager.MediaLocator.rootFolder;
		}

		public MainWindowViewModel()
		{
			MusicManager.MediaLocator.rootFolderChanged += MediaLocator_rootFolderChanged;	
		}

		private void MediaLocator_rootFolderChanged()
		{
			this.RaisePropertyChanged(nameof(RootFolder));
		}
	}
}
