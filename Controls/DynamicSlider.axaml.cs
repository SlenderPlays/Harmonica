using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Harmonica.Controls
{
	public class DynamicSlider : RangeBase
	{
		private Border? _indicator;

		// Register a new property designed specifically to hold a CornerRadius value.
		// This isn't used for the sake of having a different property, as much as it is a container.
		// The main Border (the parent of the _indicator object) has this:
		//
		//		CornerRadius="{TemplateBinding CornerRadius}">
		//
		// But because the _indicator is put above it (as a child) we get this effect:
		//
		//      ┌────────┬─────────────╮
		//      │        │             │
		//      └────────┴─────────────╯
		//
		// This isn't wanted. If we give the _indicator the same CornerRadius we get something similar
		//
		//      ╭────────╮─────────────╮
		//      │        │             │
		//      ╰────────╯─────────────╯
		//
		// The compromise was to create another property, IndicatorRadiusProperty and put it on the _indicator object
		//
		//		CornerRadius="{TemplateBinding IndicatorRadius}
		//

		/// <summary>
		/// A container-property which holds the radius of the indicator. It auto-updates when the CornerRadius property is set.
		/// You shouldn't have to set it manually.
		/// </summary>
		public static readonly StyledProperty<CornerRadius> IndicatorRadiusProperty =
		   AvaloniaProperty.Register<DynamicSlider, CornerRadius>("IndicatorRadius");

		/// <summary>
		/// A container-property which holds the radius of the indicator. It auto-updates when the CornerRadius property is set.
		/// You shouldn't have to set it manually.
		/// </summary>
		public CornerRadius IndicatorRadius
		{
			get { return GetValue(IndicatorRadiusProperty); }
			private set { SetValue(IndicatorRadiusProperty, value); }
		}

		// To avoid setting the IndicatorRadiusProperty manually, we highjack the CornerRadiusProperty
		// and add us as its owner. We also highjack the setter to also set the IndicatorProperty to the same thing,
		// but the right-hand side (the part inside) is left untouched.

		/// <inheritdoc/>
		public static new readonly StyledProperty<CornerRadius> CornerRadiusProperty =
			Border.CornerRadiusProperty.AddOwner<DynamicSlider>();


		/// <inheritdoc/>
		public new CornerRadius CornerRadius
		{
			get { return GetValue(CornerRadiusProperty); }
			set
			{
				SetValue(CornerRadiusProperty, value);
				SetValue(IndicatorRadiusProperty, new CornerRadius(value.TopLeft, 0, 0, value.BottomRight));
			}
		}


		// TODO: add vertical orientation?
		//public static readonly StyledProperty<Orientation> OrientationProperty =
		//	AvaloniaProperty.Register<ProgressBar, Orientation>(nameof(Orientation), Orientation.Horizontal);

		static DynamicSlider()
		{
			ValueProperty.Changed.AddClassHandler<DynamicSlider>((x, e) => x.UpdateIndicatorWhenPropChanged(e));
			MinimumProperty.Changed.AddClassHandler<DynamicSlider>((x, e) => x.UpdateIndicatorWhenPropChanged(e));
			MaximumProperty.Changed.AddClassHandler<DynamicSlider>((x, e) => x.UpdateIndicatorWhenPropChanged(e));
		}

		public DynamicSlider()
		{
			//UpdatePseudoClasses(IsIndeterminate, Orientation);
		}

		// Whenever the DynamicSlider is close to being rendered, we then update the size of the indicator.
		// This is used because the first time the object is rendered, the Bounds property is {0,0,0,0}.
		// This causes the indicator to not be properly-sized, and the way to get around this is to first wait for
		// the bounds property to be calculated then update de indicator. This override does that.
		protected override Size ArrangeOverride(Size finalSize)
		{
			UpdateIndicator(finalSize);

			return base.ArrangeOverride(finalSize);
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
