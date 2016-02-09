using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps;
using Xamarin.Forms.Maps;
using System.Linq;
using XFMapExtensions;

[assembly: ResolutionGroupName ("XFMapExtensions")]
[assembly: ExportEffect (typeof (XFMapExtensions.Platform.Droid.MapEffect), "MapEffect")]

namespace XFMapExtensions.Platform.Droid
{
	public class MapEffect : PlatformEffect, MapExtensionBehavior.IPlatformMapEffect
	{
		private GoogleMap googleMap;
		private MapExtensionBehavior behavior;

		#region implemented abstract members of Effect

		protected override void OnAttached()
		{
			var mapView = Control as MapView;
			if(mapView == null)
				return;

			behavior = (MapExtensionBehavior) (Element as Map)?.Behaviors?.FirstOrDefault(x =>
				x is MapExtensionBehavior);
			if(behavior == null)
				return;
			

			var zoomButton = mapView.FindViewById(1); // ズームボタン LinearLayout
			if(zoomButton != null)
			{
				zoomButton.Visibility = Android.Views.ViewStates.Invisible;
			}

			var locationButton = mapView.FindViewById(2); // 現在位置ボタン ImageView
			if(locationButton != null)
			{
				locationButton.Visibility = Android.Views.ViewStates.Invisible;
			}

			var callback = new OnMapReadyCallback();
			callback.MapReady += (sender, e) => {
				googleMap = callback.GoogleMap;
				googleMap.MyLocationChange += (sender2, e2) => {
					behavior.UserLocation = new Position(
						e2.Location.Latitude,
						e2.Location.Longitude);
				};
			};
			mapView.GetMapAsync(callback);
		}

		protected override void OnDetached()
		{
		}

		#endregion

		class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
		{
			public GoogleMap GoogleMap
			{
				get;
				private set;
			}

			public event EventHandler MapReady;

			public void OnMapReady(GoogleMap googleMap)
			{
				GoogleMap = googleMap;
				MapReady?.Invoke(this, new EventArgs());
			}
		}

		#region IAttachedPropertyChanged implementation

		public void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
			Console.WriteLine("[MapEffect.Droid]OnAttachedPropertyChanged:" + e.PropertyName);

		}

		#endregion
	}
}

