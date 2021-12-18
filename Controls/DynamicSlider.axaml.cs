using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using DynamicData;
using System;

namespace Harmonica.Controls
{
	public class DynamicSlider : RangeBase
	{
		#region Properties

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

		public static readonly StyledProperty<int> HandleRadiusProperty =
		   AvaloniaProperty.Register<DynamicSlider, int>("HandleRadius");

		public int HandleRadius
		{
			get { return GetValue(HandleRadiusProperty); }
			set { SetValue(HandleRadiusProperty, value); }
		}

		public static readonly StyledProperty<IBrush> HandleColorProperty =
		   AvaloniaProperty.Register<DynamicSlider, IBrush>("HandleColor", new SolidColorBrush(new Color(255, 0, 0, 0)));

		public IBrush HandleColor
		{
			get { return GetValue(HandleColorProperty); }
			set { SetValue(HandleColorProperty, value); }

		}

		// TODO: add vertical orientation?
		//public static readonly StyledProperty<Orientation> OrientationProperty =
		//	AvaloniaProperty.Register<ProgressBar, Orientation>(nameof(Orientation), Orientation.Horizontal);

		#endregion

		private Border? _backgroundTrack;
		private Border? _indicatorTrack;
		private Ellipse? _handle;
		private bool _isDragging = false;

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
			
			if (_indicatorTrack != null)
			{
				_indicatorTrack.ClipToBounds = true;
				var clippingMask = new GeometryGroup();

				var leftMaxCornerRadius = Math.Max(CornerRadius.TopLeft, CornerRadius.BottomLeft);
				var rightMaxCornerRadius = Math.Max(CornerRadius.TopRight, CornerRadius.BottomRight);

				var leftCornerOffset = Math.Min(Height / 2, leftMaxCornerRadius);
				var rightCornerOffset = Math.Min(Height / 2, rightMaxCornerRadius);

				clippingMask.Children?.Add(new RectangleGeometry(new Rect(
					leftCornerOffset, 0,
					finalSize.Width - leftCornerOffset - rightCornerOffset, finalSize.Height
				)));

				PathGeometry leftMask = new PathGeometry();
				PathFigure figure = new PathFigure();
				figure.IsClosed = true;
				PathSegments pathSegments = new PathSegments();
				pathSegments.Add( new BezierSegment() { Point1 = {0,0}});
				figure.Segments = pathSegments;

				leftMask.Figures.Add(figure);

				//clippingMask.Children?.Add(leftEllipse);
				//clippingMask.Children?.Add(rightEllipse);


				_indicatorTrack.Clip = clippingMask;
			}

			return base.ArrangeOverride(finalSize);
		}

		protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
		{
			_indicatorTrack = e.NameScope.Get<Border>("PART_Indicator");
			_backgroundTrack = e.NameScope.Get<Border>("PART_BackgroundTrack");
			_handle = e.NameScope.Get<Ellipse>("PART_Handle");

			// Handles left and right arrow when the backround track (effectively whole slider) is focused
			_backgroundTrack.KeyDown += BackroundTrack_KeyDown;
			// Handles any mouse presses on the slider, be it the handle itself or one of the tracks
			_handle.PointerPressed += MouseDown;
			_handle.PointerReleased += MouseUp;
			_handle.PointerMoved += MouseMoved;

			_indicatorTrack.PointerPressed += MouseDown;
			_indicatorTrack.PointerReleased += MouseUp;
			_indicatorTrack.PointerMoved += MouseMoved;

			_backgroundTrack.PointerPressed += MouseDown;
			_backgroundTrack.PointerReleased += MouseUp;
			_backgroundTrack.PointerMoved += MouseMoved;


			

			UpdateIndicator(Bounds.Size);
		}

		// TODO: fix bug where the handle doesn't set itself to visible beacuse REASONS???
		// It is set, but then next time this method is called, _handle.IsVisble is still set to true.
		// This only happens if it has never been clicked on...
		protected override void OnPointerEnter(Avalonia.Input.PointerEventArgs e)
		{
			if(_handle != null) _handle.IsVisible = true;
			base.OnPointerEnter(e);
		}
		protected override void OnPointerLeave(Avalonia.Input.PointerEventArgs e)
		{
			if (_handle != null) _handle.IsVisible = false;
			base.OnPointerLeave(e);
		}

		private void BackroundTrack_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
		{
			double increment = (Maximum - Minimum) / 100;
			if (EnumExtensions.HasAllFlags(e.KeyModifiers, Avalonia.Input.KeyModifiers.Shift))
			{
				increment *= 10;
			}

			if (e.Key == Avalonia.Input.Key.Right)
			{
				Value += increment;
				e.Handled = true;
			}
			else if (e.Key == Avalonia.Input.Key.Left)
			{
				Value -= increment;
				e.Handled = true;
			}
		}

		private void MouseDown(object? sender, Avalonia.Input.PointerPressedEventArgs e)
		{
			_isDragging = true;
			MouseMoved(this, e);
			e.Handled = true;
		}
		
		private void MouseUp(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
		{
			_isDragging = false;
			e.Handled = true;
		}

		private void MouseMoved(object? sender, Avalonia.Input.PointerEventArgs e)
		{
			if (!_isDragging) return;
			if (_backgroundTrack == null) return;

			var point = e.GetCurrentPoint(_backgroundTrack);
			
			// Equals 0 (or less) if the mouse is moved to the left most part of the background track
			// and equals the background track's length if it is at the other extreme.
			double size = point.Position.X;
			double maxSize = _backgroundTrack.Bounds.Size.Width;
		
			if (maxSize != 0)
			{
				// Remap from the given value from [0,1] to [Minimum, Maximum]
				double percent = size / maxSize;
				Value = Minimum + percent * (Maximum - Minimum);
			}

			e.Handled = true;
		}



		private void UpdateIndicator(Size bounds)
		{
			if (_indicatorTrack != null)
			{
				double percent = Maximum == Minimum ? 1.0 : (Value - Minimum) / (Maximum - Minimum);
				_indicatorTrack.Width = bounds.Width * percent;
			}
		}

        private void UpdateIndicatorWhenPropChanged(AvaloniaPropertyChangedEventArgs e)
		{
			UpdateIndicator(Bounds.Size);
		}
	}
}
