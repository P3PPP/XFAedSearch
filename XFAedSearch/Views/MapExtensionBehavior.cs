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

		protected override void OnAttachedTo(Map bindable)
		{
			base.OnAttachedTo(bindable);

			if(bindable as Map == null)
				return;

			associated = bindable;
			BindingContext = bindable.BindingContext;
			bindable.BindingContextChanged += Bindable_BindingContextChanged;

			mapEffect = new MapEffect();
			toNotify = InnerField.GetValue(mapEffect) as IPlatformMapEffect;
			toNotify.MapLoaded += (sender, e) => this.MapLoaded?.Invoke(this, new EventArgs());
//			toNotify.MapTapped += (sender, e) => this.MapTapped?.Invoke(this, new EventArgs());

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

		public event EventHandler MapLoaded;
//		public event EventHandler MapTapped;

		#region BindableProperties

		public static readonly BindableProperty UserLocationProperty =
			BindableProperty.Create("UserLocation", typeof(Position?), typeof(MapExtensionBehavior), default(Position?),
				propertyChanged: (bindable, oldValue, newValue) =>
				((MapExtensionBehavior)bindable).OnPropertyChanged(UserLocationProperty.PropertyName));

		public Position? UserLocation
		{
			get { return (Position?)GetValue(UserLocationProperty); }
			set { SetValue(UserLocationProperty, value); }
		}

		#endregion

		private void OnPropertyChanged(string propertyName)
		{
			toNotify?.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}


		public interface IPlatformMapEffect
		{
			void OnPropertyChanged(PropertyChangedEventArgs e);
			event EventHandler MapLoaded;
//			event EventHandler MapTapped;
		}

		private class MapEffect : RoutingEffect
		{
			public MapEffect() : base ("XFMapExtensions.MapEffect")
			{
			}
		}
	}
}

