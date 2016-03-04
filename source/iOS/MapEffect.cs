using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.iOS;
using MapKit;
using CoreLocation;
using Foundation;
using XFMapExtensions;
using UIKit;

[assembly: ResolutionGroupName ("XFMapExtensions")]
[assembly: ExportEffect (typeof (XFMapExtensions.Platform.iOS.MapEffect), "MapEffect")]

namespace XFMapExtensions.Platform.iOS
{
	public class MapEffect : PlatformEffect, MapExtensionBehavior.IPlatformMapEffect
	{
		private MKMapView mapView;
		private MKPolygon polygon;
		private MKPolygonRenderer polygonRenderer;
		private MapExtensionBehavior behavior;

//		private UITapGestureRecognizer tapGesture;
		private bool isFirstTime = true;

		#region implemented abstract members of Effect

		protected override void OnAttached()
		{
			mapView = Control as MKMapView;
			if(mapView == null)
				return;

			behavior = (MapExtensionBehavior) (Element as Map)?.Behaviors?.FirstOrDefault(x =>
				x is MapExtensionBehavior);
			if(behavior == null)
				return;

//			tapGesture = new UITapGestureRecognizer(() => 
//			{
//				Console.WriteLine("MapViewTapped");
//				MapTapped?.Invoke(this, new EventArgs());
//			})
//			{
//				NumberOfTapsRequired = 1,
//
//			};
//			mapView.AddGestureRecognizer(tapGesture);

			mapView.DidUpdateUserLocation += DidUpdateUserLocation;

			// iOSのMapLoadedは新しいエリアを読み込んだ時に発火する(Androidとは違うっぽい)
//			mapView.MapLoaded += MapView_MapLoaded;


//			mapView.OverlayRenderer = (mapView, overlay) =>
//			{
//				if(polygonRenderer == null)
//				{
//					polygonRenderer = new MKPolygonRenderer (overlay as MKPolygon);
//					polygonRenderer.FillColor = UIKit.UIColor.Blue;
//					polygonRenderer.StrokeColor = UIKit.UIColor.Red;
//				}
//				return polygonRenderer;
//			};
		}

		void MapView_MapLoaded (object sender, EventArgs e)
		{
			MapLoaded?.Invoke(this, new EventArgs());
		}

		private void DidUpdateUserLocation (object sender, MKUserLocationEventArgs e)
		{
			if(e.UserLocation == null)
				return;

			behavior.UserLocation = new Position(
				e.UserLocation.Coordinate.Latitude,
				e.UserLocation.Coordinate.Longitude);

			// 暫定的にユーザー位置が初めて更新された時をロード完了として扱う
			if(isFirstTime)
			{
				isFirstTime = false;
				MapLoaded?.Invoke(behavior, new EventArgs());
			}
		}

		protected override void OnDetached()
		{
			mapView.DidUpdateUserLocation -= DidUpdateUserLocation;
			mapView.MapLoaded -= MapView_MapLoaded;

//			mapView.RemoveGestureRecognizer(tapGesture);
//			tapGesture.Dispose();
//			tapGesture = null;

			mapView = null;
			behavior = null;
			polygon?.Dispose();
			polygon = null;
			polygonRenderer?.Dispose();
			polygonRenderer = null;
		}

		#endregion

		#region IAttachedPropertyChanged implementation

		public void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
		{
//			Console.WriteLine("[MapEffect.iOS]OnAttachedPropertyChanged:" + e.PropertyName);
//			if(e.PropertyName != MapExtensionBehavior.UserLocationProperty.PropertyName)
//				return;

//			if(e.PropertyName != XFAedSearch.Views.MapExtendBehavior.PositionsProperty.PropertyName)
//			{
//				UpdatePolygon();
//				return;
//			}
		}

		#endregion

		private void UpdatePolygon()
		{
			if(mapView?.UserLocation?.Coordinate == null)
				return;

			if(polygon != null)
			{
				mapView.RemoveOverlay(polygon);
				polygon.Dispose();
			}

			polygon = MKPolygon.FromCoordinates(new[] {
				mapView.UserLocation.Coordinate,
				new CLLocationCoordinate2D(mapView.UserLocation.Coordinate.Latitude + 5, mapView.UserLocation.Coordinate.Longitude + 5),
				new CLLocationCoordinate2D(mapView.UserLocation.Coordinate.Latitude - 5, mapView.UserLocation.Coordinate.Longitude + 3),
			});

			mapView.AddOverlay(polygon);
		}

		public event EventHandler MapLoaded;
//		public event EventHandler MapTapped;
	}

}

