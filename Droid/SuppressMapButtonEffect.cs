using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps;

[assembly: ResolutionGroupName ("P3PPP")]
[assembly: ExportEffect (typeof (XFAedSearch.Droid.SuppressMapButtonEffect), "SuppressMapButtonEffect")]

namespace XFAedSearch.Droid
{
	public class SuppressMapButtonEffect : PlatformEffect
	{
		#region implemented abstract members of Effect

		protected override void OnAttached()
		{
			var mapView = Control as MapView;
			if(mapView == null)
				return;

			var zoomButton = mapView.FindViewById(1); // ズームボタン LinearLayout
			if(zoomButton != null)
			{
				zoomButton.Visibility = Android.Views.ViewStates.Gone;
			}

			var locationButton = mapView.FindViewById(2); // 現在位置ボタン ImageView
			if(locationButton != null)
			{
				locationButton.Visibility = Android.Views.ViewStates.Gone;
			}
		}

		protected override void OnDetached()
		{
		}

		#endregion
	}
}

