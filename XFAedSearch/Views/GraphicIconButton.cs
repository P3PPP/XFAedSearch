using System;
using System.Collections.Generic;
using NControl.Abstractions;
using NGraphics;
using Xamarin.Forms;
using System.Windows.Input;

namespace XFAedSearch.Views
{
	public class GraphicIconButton : NControlView
	{
		private readonly NControlView icon;
		private readonly NControlView circle;

		public GraphicIconButton()
		{
			HeightRequest = 44;
			WidthRequest = 44;

			icon = new NControlView {
				DrawingFunction = DrawIcon,
			};

			circle = new NControlView {
				DrawingFunction = DrawCircle,
			};

			Content = new Grid {
				Children = {
					circle,
					icon,
				},
			};

			OnInvalidate += (sender, e) => 
			{
				circle.Invalidate();
				icon.Invalidate();
			};
		}

		protected override void OnPropertyChanging(string propertyName)
		{
			// [iOS]IsVisible=falseなLayoutにNControlViewを追加すると
			// IsVisible=trueになった時にDrawingFunctionが呼ばれない件への対処

			if(propertyName == "Parent" &&
				Parent != null)
			{
				Parent.PropertyChanged -= ParentPropertyChanged;
			}
			base.OnPropertyChanged(propertyName);
		}

		protected override void OnPropertyChanged(string propertyName)
		{
			// [iOS]IsVisible=falseなLayoutにNControlViewを追加すると
			// IsVisible=trueになった時にDrawingFunctionが呼ばれない件への対処

			if(propertyName == "Parent")
			{
				Parent.PropertyChanged += ParentPropertyChanged;
			}
			base.OnPropertyChanged(propertyName);
		}

		void ParentPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// [iOS]IsVisible=falseなLayoutにNControlViewを追加すると
			// IsVisible=trueになった時にDrawingFunctionが呼ばれない件への対処

			if(e.PropertyName == "IsVisible" && 
				true.Equals((Parent as VisualElement).IsVisible))
			{
				Invalidate();
			}
		}

		private void DrawCircle(ICanvas canvas, NGraphics.Rect rect)
		{
			var color = new NGraphics.Color(BackColor.R,
				BackColor.G, BackColor.B, BackColor.A);
			canvas.FillEllipse(rect, color);
		}

		private void DrawIcon(ICanvas canvas, NGraphics.Rect rect)
		{
			if(Graphic == null)
				return;

			var padding = ((rect.Size * IconScale) - rect.Size) / 2;
			var transform = Transform.AspectFillRect(Graphic.ViewBox, rect.GetInflated(padding));
			var transformed = Graphic.TransformGeometry(transform);

			var color = new NGraphics.Color(
				FillColor.R,
				FillColor.G,
				FillColor.B,
				FillColor.A);

			foreach(var element in transformed.Children)
			{
				if(OverpaintEnabled)
				{
					ApplyColor(element, color);
				}
				element.Draw(canvas);
			}
		}

		private void ApplyColor(NGraphics.Element element, NGraphics.Color color)
		{
			var children = (element as Group)?.Children;
			if(children != null)
			{
				foreach(var child in children)
				{
					ApplyColor(child, color);
				}
			}

			if(element?.Pen != null)
			{
				element.Pen = new Pen(color, element.Pen.Width);
			}

			if(element?.Brush != null)
			{
				element.Brush = new SolidBrush(color);
			}
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
				typeof(GraphicIconButton),
				null);

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); } 
		}

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create("CommandParameter", typeof(object),
				typeof(GraphicIconButton),
				default(object));

		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); } 
		}

		public static readonly BindableProperty FillColorProperty =
			BindableProperty.Create("FillColor", typeof(Xamarin.Forms.Color),
				typeof(GraphicIconButton),
				Xamarin.Forms.Color.Black,
				propertyChanged:(bindable, oldValue, newValue) => ((GraphicIconButton)bindable).icon.Invalidate());

		public Xamarin.Forms.Color FillColor
		{
			get { return (Xamarin.Forms.Color)GetValue(FillColorProperty); }
			set { SetValue(FillColorProperty, value); } 
		}

		public static readonly BindableProperty BackColorProperty =
			BindableProperty.Create("BackColor", typeof(Xamarin.Forms.Color),
				typeof(GraphicIconButton),
				Xamarin.Forms.Color.White,
				propertyChanged:(bindable, oldValue, newValue) => ((GraphicIconButton)bindable).circle.Invalidate());

		public Xamarin.Forms.Color BackColor
		{
			get { return (Xamarin.Forms.Color)GetValue(BackColorProperty); }
			set { SetValue(BackColorProperty, value); } 
		}

		public static readonly BindableProperty IconScaleProperty =
			BindableProperty.Create("IconScale", typeof(double),
				typeof(GraphicIconButton),
				1.0,
				propertyChanged:(bindable, oldValue, newValue) => ((GraphicIconButton)bindable).icon.Invalidate());

		public double IconScale
		{
			get { return (double)GetValue(IconScaleProperty); }
			set { SetValue(IconScaleProperty, value); } 
		}

		public static readonly BindableProperty OverpaintEnabledProperty =
			BindableProperty.Create("OverpaintEnabled", typeof(bool),
				typeof(GraphicIconButton),
				false,
				propertyChanged:(bindable, oldValue, newValue) => ((GraphicIconButton)bindable).icon.Invalidate());

		public bool OverpaintEnabled
		{
			get { return (bool)GetValue(OverpaintEnabledProperty); }
			set { SetValue(OverpaintEnabledProperty, value); } 
		}

		public static readonly BindableProperty GraphicProperty =
			BindableProperty.Create("Graphic", typeof(Graphic),
				typeof(GraphicIconButton),
				null,
				propertyChanged:(bindable, oldValue, newValue) => ((GraphicIconButton)bindable).icon.Invalidate());

		public Graphic Graphic
		{
			get { return (Graphic)GetValue(GraphicProperty); }
			set { SetValue(GraphicProperty, value); } 
		}
		#endregion
	}
}

