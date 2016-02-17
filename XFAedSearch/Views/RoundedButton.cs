using System;
using System.Collections.Generic;
using NControl.Abstractions;
using NGraphics;
using Xamarin.Forms;
using System.Windows.Input;

namespace XFAedSearch.Views
{
	public class RoundedButton : NControlView
	{
		private Label label;

		public RoundedButton()
		{
			label = new Label {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
				VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center,
				FontSize = FontSize,
				LineBreakMode = LineBreakMode.NoWrap,
			};
			label.PropertyChanged += (sender, e) => 
			{
				if(e.PropertyName == Label.TextProperty.PropertyName)
				{
					InvalidateLayout();
				}
			};
			Content = label;
			SizeChanged += (sender, e) => 
			{
				Invalidate();
			};
		}

		public override void Draw(ICanvas canvas, Rect rect)
		{
			if(DrawingFunction != null)
			{
				base.Draw(canvas, rect);
				return;
			}

			var borderColro = NGraphics.Color.FromHSL(
				                  BorderColor.Hue,
				                  BorderColor.Saturation,
				                  BorderColor.Luminosity,
				                  BorderColor.A);
			var pen = new Pen(borderColro, BorderWidth);
			var fillColor = NGraphics.Color.FromHSL(
				                BackColor.Hue,
				                BackColor.Saturation,
				                BackColor.Luminosity,
				                BackColor.A);
			var brush = new SolidBrush(fillColor);

			var padding = BorderWidth <= 0 ? 0.0 : BorderWidth;
			var newRect = rect.GetInflated(-padding);
			var curveSize = newRect.Height / 2;

			canvas.DrawPath(new PathOp []{ 
				new MoveTo(newRect.Left+curveSize, newRect.Top),
				new LineTo(newRect.Right-curveSize, newRect.Top),
				new CurveTo(
					new NGraphics.Point(newRect.Right-curveSize, newRect.Top),
					newRect.TopRight,
					new NGraphics.Point(newRect.Right, newRect.Top+curveSize)
				),
				new CurveTo(
					new NGraphics.Point(newRect.Right, newRect.Bottom-curveSize),
					newRect.BottomRight,
					new NGraphics.Point(newRect.Right-curveSize, newRect.Bottom)
				),
				new LineTo(newRect.Left+curveSize, newRect.Bottom),
				new CurveTo(
					new NGraphics.Point(newRect.Left+curveSize, newRect.Bottom),
					newRect.BottomLeft,
					new NGraphics.Point(newRect.Left, newRect.Bottom-curveSize)
				),
				new CurveTo(
					new NGraphics.Point(newRect.Left, newRect.Top+curveSize),
					newRect.TopLeft,
					new NGraphics.Point(newRect.Left+curveSize, newRect.Top)
				),
				new ClosePath()
			}, pen, brush);
		}

		private void OnTextChanged()
		{
			label.Text = Text;
		}

		private void OnTextColorChanged()
		{
			label.TextColor = TextColor;
		}

		private void OnFontSizeChanged()
		{
			label.FontSize = FontSize;
		}

		public override bool TouchesBegan(IEnumerable<NGraphics.Point> points)
		{
			base.TouchesBegan(points);
			this.ScaleTo(0.9, 65, Easing.CubicInOut);
			return true;
		}

		public override bool TouchesCancelled(IEnumerable<NGraphics.Point> points)
		{
			base.TouchesCancelled(points);
			this.ScaleTo(1.0, 65, Easing.CubicInOut);
			return true;
		}

		public override bool TouchesEnded(IEnumerable<NGraphics.Point> points)
		{
			base.TouchesEnded(points);
			this.ScaleTo(1.0, 65, Easing.CubicInOut);
			Clicked?.Invoke(this, new EventArgs());
			if (Command != null && Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);

			return true;
		}

		public event EventHandler Clicked;	

		#region BindableProperties
		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create("Command", typeof(ICommand),
				typeof(RoundedButton),
				null);

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); } 
		}

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create("CommandParameter", typeof(object),
				typeof(RoundedButton),
				default(object));

		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); } 
		}

		public static readonly BindableProperty BorderColorProperty =
			BindableProperty.Create("BorderColor", typeof(Xamarin.Forms.Color),
				typeof(RoundedButton),
				Xamarin.Forms.Color.Black,
				propertyChanged:(bindable, oldValue, newValue) => ((RoundedButton)bindable).Invalidate());

		public Xamarin.Forms.Color BorderColor
		{
			get { return (Xamarin.Forms.Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); } 
		}

		public static readonly BindableProperty BorderWidthProperty =
			BindableProperty.Create("BorderWidth", typeof(double),
				typeof(RoundedButton),
				0.0,
				propertyChanged:(bindable, oldValue, newValue) => ((RoundedButton)bindable).Invalidate());

		public double BorderWidth
		{
			get { return (double)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); } 
		}

		public static readonly BindableProperty BackColorProperty =
			BindableProperty.Create("BackColor", typeof(Xamarin.Forms.Color),
				typeof(RoundedButton),
				Xamarin.Forms.Color.White,
				propertyChanged:(bindable, oldValue, newValue) => ((RoundedButton)bindable).Invalidate());

		public Xamarin.Forms.Color BackColor
		{
			get { return (Xamarin.Forms.Color)GetValue(BackColorProperty); }
			set { SetValue(BackColorProperty, value); } 
		}

		public static readonly BindableProperty TextProperty =
			BindableProperty.Create("Text", typeof(string),
				typeof(RoundedButton),
				null,
				propertyChanged:(bindable, oldValue, newValue) => ((RoundedButton)bindable).OnTextChanged());

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); } 
		}

		public static readonly BindableProperty TextColorProperty =
			BindableProperty.Create("TextColor", typeof(Xamarin.Forms.Color),
				typeof(RoundedButton),
				Xamarin.Forms.Color.Black,
				propertyChanged:(bindable, oldValue, newValue) => ((RoundedButton)bindable).OnTextColorChanged());

		[TypeConverter(typeof(ColorTypeConverter))]
		public Xamarin.Forms.Color TextColor
		{
			get { return (Xamarin.Forms.Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); } 
		}

		public static readonly BindableProperty FontSizeProperty =
			BindableProperty.Create("FontSize", typeof(double),
				typeof(RoundedButton),
				Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				propertyChanged:(bindable, oldValue, newValue) => ((RoundedButton)bindable).OnFontSizeChanged());

		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); } 
		}

		#endregion
	}
}


