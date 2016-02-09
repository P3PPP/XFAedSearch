using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace XFMapExtensions
{
	public class MapExtensionBehavior : Behavior<Map>
	{
		private static FieldInfo InnerField = typeof(RoutingEffect)
			.GetField("Inner", BindingFlags.NonPublic | BindingFlags.Instance);

		private MapEffect mapEffect;
		private IPlatformMapEffect toNotify;
		private Map associated;

		public MapExtensionBehavior()
		{
		}

		protected override void OnAttachedTo(Map bindable)
		{
			base.OnAttachedTo(bindable);

			associated = bindable;
			BindingContext = bindable.BindingContext;
			bindable.BindingContextChanged += Bindable_BindingContextChanged;

			mapEffect = new MapEffect();
			toNotify = InnerField.GetValue(mapEffect) as IPlatformMapEffect;

			bindable.Effects.Add (mapEffect);

		}

		protected override void OnDetachingFrom(Map bindable)
		{
			bindable.BindingContextChanged -= Bindable_BindingContextChanged;
			BindingContext = null;
			associated = null;
			bindable.Effects.Remove(mapEffect);
			mapEffect = null;
			toNotify = null;

			base.OnDetachingFrom(bindable);
		}

		void Bindable_BindingContextChanged (object sender, EventArgs e)
		{
			BindingContext = associated.BindingContext;
		}


		#region BindableProperties

		public static readonly BindableProperty UserLocationProperty =
			BindableProperty.Create("UserLocation", typeof(Position), typeof(MapExtensionBehavior), default(Position),
				propertyChanged: (bindable, oldValue, newValue) =>
				((MapExtensionBehavior)bindable).OnAttachedPropertyChanged(UserLocationProperty.PropertyName));

		public Position UserLocation
		{
			get { return (Position)GetValue(UserLocationProperty); }
			set { SetValue(UserLocationProperty, value); }
		}

		#endregion

		private void OnAttachedPropertyChanged(string propertyName)
		{
			toNotify?.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}


		public interface IPlatformMapEffect
		{
			void OnPropertyChanged(PropertyChangedEventArgs e);
		}

		private class MapEffect : RoutingEffect
		{
			public MapEffect() : base ("XFMapExtensions.MapEffect")
			{
			}
		}
	}
}

