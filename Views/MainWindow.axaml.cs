using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LibVLCSharp.Shared;
using System;

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
			var myPanel = this.FindControl<StackPanel>("MyPanel");
			myPanel.Transitions?.Add(new DoubleTransition()
			{
				Property = StackPanel.OpacityProperty,
				Duration = TimeSpan.FromSeconds(4),
				Easing = new LinearEasing()
			}); 
			
		}
	}
}
