using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
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

				googleMap.MyLocationChange += GoogleMap_MyLocationChange;
				// なぜかCameraChangeにハンドラを追加するとXF.Maps.Map.VisibleRegionが更新されなくなる
//				googleMap.CameraChange += GoogleMap_CameraChange;

				var point = new LatLng(
					XFAedSearch.Droid.Helpers.Settings.Latitude,
					XFAedSearch.Droid.Helpers.Settings.Longitude);
				googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(point,
					XFAedSearch.Droid.Helpers.Settings.ZoomLevel));

				googleMap.SetOnMapLoadedCallback(new MapLoadedCallback {
					OnMapLoadedAction = () => MapLoaded?.Invoke(this, new EventArgs())
				});
			};
			mapView.GetMapAsync(callback);

		}

		protected override void OnDetached()
		{
			if(googleMap != null)
			{
				googleMap.MyLocationChange -= GoogleMap_MyLocationChange;
				googleMap.CameraChange -= GoogleMap_CameraChange;

				var position = googleMap.CameraPosition;
				XFAedSearch.Droid.Helpers.Settings.Latitude = position.Target.Latitude;
				XFAedSearch.Droid.Helpers.Settings.Longitude = position.Target.Longitude;
				XFAedSearch.Droid.Helpers.Settings.ZoomLevel = position.Zoom;
			}
		}

		void GoogleMap_MyLocationChange (object sender, GoogleMap.MyLocationChangeEventArgs e)
		{
			behavior.UserLocation = new Position(
				e.Location.Latitude,
				e.Location.Longitude);

			// なぜかCameraChangeにハンドラを追加するとXF.Maps.Map.VisibleRegionが更新されなくなるので、暫定的にここで対処
			var position = googleMap.CameraPosition;
			XFAedSearch.Droid.Helpers.Settings.Latitude = position.Target.Latitude;
			XFAedSearch.Droid.Helpers.Settings.Longitude = position.Target.Longitude;
			XFAedSearch.Droid.Helpers.Settings.ZoomLevel = position.Zoom;
		}

		void GoogleMap_CameraChange (object sender, GoogleMap.CameraChangeEventArgs e)
		{
			XFAedSearch.Droid.Helpers.Settings.Latitude = e.Position.Target.Latitude;
			XFAedSearch.Droid.Helpers.Settings.Longitude = e.Position.Target.Longitude;
			XFAedSearch.Droid.Helpers.Settings.ZoomLevel = e.Position.Zoom;
		}



		#endregion

		class MapLoadedCallback : Java.Lang.Object, GoogleMap.IOnMapLoadedCallback
		{
			public Action OnMapLoadedAction
			{
				get;
				set;
			}

			public void OnMapLoaded()
			{
				OnMapLoadedAction?.Invoke();
			}
		}


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

		public event EventHandler MapLoaded;
	}
}

