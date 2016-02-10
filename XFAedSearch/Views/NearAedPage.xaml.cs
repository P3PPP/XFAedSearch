using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

using AedOpenDataApiWrapper;
using System.Threading.Tasks;

using XFAedSearch.Models;
using XFAedSearch.ViewModels;
using XFMapExtensions;

namespace XFAedSearch.Views
{
	public partial class NearAedPage : ContentPage
	{
		readonly string FocusToKey = "/Map/FocusTo";
		readonly string UpdateNearAedsKey = "/Map/Pins/Update/NearAeds";
		readonly string NearAedsFailedKey = "/SearchNearAeds/?Result=Failed";

		public NearAedPage ()
		{
			InitializeComponent ();

			// 中央揃えに見せるトリック
			searchButton.SizeChanged += (sender, e) => searchButton.TranslationX = -(searchButton.Width / 2);

			MessagingCenter.Subscribe<AedsViewModel, Position>(
				this,
				FocusToKey,
				(_, position) =>
			{
				Console.WriteLine(@"Message received: key=""/Map/FocusTo""");
				var radius = map?.VisibleRegion?.Radius ?? Distance.FromMeters(Settings.RegionRadius);
				map.MoveToRegion(MapSpan.FromCenterAndRadius(position, radius));
			});

			MessagingCenter.Subscribe<AedsViewModel, List<AedViewModel>>(
				this,
				UpdateNearAedsKey,
				(_, aeds) =>
			{
				Console.WriteLine(@"Message received: key=""/Map/Pins/Update/NearAeds""");
				UpdatePins(aeds);
			});

			MessagingCenter.Subscribe<NearAedPageViewModel, string>(
				this,
				NearAedsFailedKey,
				(_, resourceKey) =>
			{
				Console.WriteLine(@"Message received: key=""/SearchNearAeds/?Result=Failed""");
				object value;
				Application.Current.Resources.TryGetValue(resourceKey, out value);
				var message = value as string;
				DisplayAlert("付近のAED検索", message, "OK");
			});
		}

		public void MoveToReagion(MapSpan mapspan) => map.MoveToRegion(mapspan);

		private void MoveToCurrentPositionButtonClicked(object sender, EventArgs e)
		{
			var userLocation = mapExBehavior.UserLocation;
			map.MoveToRegion(MapSpan.FromCenterAndRadius(
				userLocation, map.VisibleRegion.Radius));
			return;
		}

		private async void FlyOutButtonClicked(object sender, EventArgs e)
		{
			if(flyOut.IsVisible)
			{
				await flyOut.TranslateTo(0, flyOut.Height, 300);
				flyOut.IsVisible = !flyOut.IsVisible;
			}
			else
			{
				await flyOut.TranslateTo(0, flyOut.Height, 0);
				flyOut.IsVisible = !flyOut.IsVisible;
				await flyOut.TranslateTo(0, 0, 300);
			}
		}

		private void HamburgerButtonClicked(object sender, EventArgs e)
		{
			var masterDetail = this.Parent as MasterDetailPage;
			if(masterDetail == null)
				return;

			masterDetail.IsPresented = !masterDetail.IsPresented;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// 前回マップに表示していた位置を復元
			map.MoveToRegion(MapSpan.FromCenterAndRadius(
				new Position(Settings.RegionLatitude, Settings.RegionLongitude),
				Distance.FromMeters(Settings.RegionRadius)));
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var aeds = (BindingContext as NearAedPageViewModel)?.AedsViewModel?.Value?.Aeds;
			if(aeds != null )
			{
				UpdatePins(aeds);
			}
		}

		protected override void OnDisappearing()
		{
			map.Behaviors.Clear();
			MessagingCenter.Unsubscribe<AedsViewModel, Position>(this, FocusToKey);
			MessagingCenter.Unsubscribe<AedsViewModel, List<AedInfo>>(this, UpdateNearAedsKey);
			MessagingCenter.Unsubscribe<NearAedPageViewModel, string>(this, NearAedsFailedKey);

			SaveRegion();

			base.OnDisappearing();
		}

		private void SaveRegion()
		{
			Settings.RegionLatitude = map.VisibleRegion.Center.Latitude;
			Settings.RegionLongitude = map.VisibleRegion.Center.Longitude;
			Settings.RegionRadius = map.VisibleRegion.Radius.Meters;
		}

		private void UpdatePins(List<AedViewModel> aeds)
		{
			map?.Pins?.Clear();
			aeds?.ForEach(aed => {
				var pin = new Pin {
					Type = PinType.Place,
					Position = new Position(aed.AedInfo.Latitude, aed.AedInfo.Longitude),
					Label =  aed.LocationName,
				};
				map.Pins.Add(pin);
			});
		}
	}
}

