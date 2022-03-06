using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Harmonica.Controls
{
	public class SongFolderControl : ContentControl
	{
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<SongFolderControl, string>("Title", "Untitled");

        public string Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly StyledProperty<bool> ExpandedProperty =
            AvaloniaProperty.Register<SongFolderControl, bool>("Expanded", false);

        public bool Expanded
        {
            get { return GetValue(ExpandedProperty); }
            set { SetValue(ExpandedProperty, value); }
        }
    }
}
