using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Harmonica.Controls
{
	public class DynamicSlider : RangeBase
	{
		private Border? _indicator;

		//public static readonly StyledProperty<Orientation> OrientationProperty =
		//	AvaloniaProperty.Register<ProgressBar, Orientation>(nameof(Orientation), Orientation.Horizontal);

		static DynamicSlider()
		{
			ValueProperty.Changed.AddClassHandler<DynamicSlider>((x, e) => x.UpdateIndicatorWhenPropChanged(e));
			MinimumProperty.Changed.AddClassHandler<DynamicSlider>((x, e) => x.UpdateIndicatorWhenPropChanged(e));
			MaximumProperty.Changed.AddClassHandler<DynamicSlider>((x, e) => x.UpdateIndicatorWhenPropChanged(e));
			BoundsProperty.Changed.AddClassHandler<DynamicSlider>((x, e) => x.UpdateIndicatorWhenPropChanged(e));
		}

		public DynamicSlider()
		{
			//UpdatePseudoClasses(IsIndeterminate, Orientation);
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			_indicator = e.NameScope.Get<Border>("PART_Indicator");

			UpdateIndicator(Bounds.Size);
		}

		private void UpdateIndicator(Size bounds)
		{
			if (_indicator != null)
			{
				double percent = Maximum == Minimum ? 1.0 : (Value - Minimum) / (Maximum - Minimum);
				_indicator.Width = bounds.Width * percent;
			}
		}

        private void UpdateIndicatorWhenPropChanged(AvaloniaPropertyChangedEventArgs e)
		{
			UpdateIndicator(Bounds.Size);
		}
	}
}
