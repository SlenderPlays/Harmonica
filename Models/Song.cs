using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Harmonica.Models
{
	class Song
	{
		public Song(string path)
		{
			this.FilePath = path;

			// TODO: deal with more tags
			var tagFile = TagLib.File.Create(path);

			this.WithTitle(Path.GetFileNameWithoutExtension(path))
				.WithAuthors(tagFile.Tag.Performers);
		}

		public const string DefaultTitle = "Untitled";
		public const string DefaultAuthor = "Unknown Author";

		public string FilePath { get; private set; }
		public string Title { get; set; } = DefaultTitle;
		public string[] Authors { get; set; } = { DefaultAuthor };
		//public int Length { get; set; } = -1;

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
	}
}
