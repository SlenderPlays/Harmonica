using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LibVLCSharp.Shared;
using Harmonica.Music;
using TagLib;

namespace Harmonica.Models
{
	public class Song
	{
		public Song(string path)
		{
			this.FilePath = path;

			var tagFile = TagLib.File.Create(path);

			if (tagFile.Tag.Title != null)
			{
				WithTitle(tagFile.Tag.Title);
			}
			else
			{
				WithTitle(Path.GetFileNameWithoutExtension(path));
			}
			WithAuthors(tagFile.Tag.Performers);
			WithAlbum(tagFile.Tag.Album);

			// Can't access MusicManager because this constructor is called by the MusicManager constructor
			// string relativePath = path.Replace(MusicManager.MediaLocator.musicPath, "");
			// Media media = MusicManager.MediaLocator.GetMedia(MusicManager.LibVLC, relativePath);
			// Duration = TimeSpan.FromMilliseconds(media.Duration);
			// media.Dispose();
			
			Duration = tagFile.Properties.Duration;
			if (tagFile.Tag.Pictures.Length > 0)
			{
				MemoryStream ms = new MemoryStream(tagFile.Tag.Pictures[0].Data.Data);
				ms.Seek(0, SeekOrigin.Begin);

				Thumbnail = new Avalonia.Media.Imaging.Bitmap(ms);
			}
		}

		public const string DefaultTitle = "Untitled";
		public const string DefaultAuthor = "Unknown Author";
		public const string DefaultAlbum = "No Album";

		public string FilePath { get; private set; }
		
		public string Title { get; set; } = DefaultTitle;
		public string[] Authors { get; set; } = { DefaultAuthor };
		public string Album { get; set; } = DefaultAlbum;
		
		public TimeSpan Duration { get; set; } = new TimeSpan(0, 0, 0);
		public Avalonia.Media.Imaging.Bitmap? Thumbnail { get; set; }

		public Song WithTitle(string? title)
		{
			this.Title = String.IsNullOrWhiteSpace(title) ? DefaultTitle : title;
			return this;
		}
		public Song WithAuthors(string[] authors)
		{
			this.Authors = authors.Length == 0? new string[]{DefaultAuthor} : authors;
			return this;
		}
		public Song WithAlbum(string? album)
		{
			this.Album = String.IsNullOrWhiteSpace(album) ? DefaultAlbum : album;
			return this;
		}
	}
}
