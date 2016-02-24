using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

using AedOpenDataApiWrapper;
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

		#if DEBUG
		private Label radiusLabel;
		#endif

		public NearAedPage ()
		{
			InitializeComponent ();

			// 中央揃えに見せるトリック
			searchButton.SizeChanged += (sender, e) => searchButton.TranslationX = -(searchButton.Width / 2);

			#if DEBUG
			radiusLabel = new Label{
				TextColor = Color.Black,
				BackgroundColor = Color.Silver.MultiplyAlpha(0.2),
				InputTransparent = true,
			};
			relativeLayout.Children.Add(radiusLabel,
				() => relativeLayout.Width / 2,
				() => relativeLayout.Height / 2);
			radiusLabel.SizeChanged += (sender, e) =>
			{
				radiusLabel.TranslationY = -(radiusLabel.Height / 2);
				radiusLabel.TranslationX = -(radiusLabel.Width / 2);
			};
			radiusLabel.Text = Settings.RegionRadius.ToString();
			#endif

			// 前回マップに表示していた位置を復元
			map.MoveToRegion(MapSpan.FromCenterAndRadius(
				new Position(Settings.RegionLatitude, Settings.RegionLongitude),
				Distance.FromMeters(Settings.RegionRadius)));
		}

		public void MoveToReagion(MapSpan mapspan) => map.MoveToRegion(mapspan);

		protected override void OnAppearing()
		{
			base.OnAppearing();

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

			if(!map.Behaviors.Contains(mapExBehavior))
			{
				map.Behaviors.Add(mapExBehavior);
			}
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			var vm = (BindingContext as NearAedPageViewModel);
			if(vm == null)
				return;
			
			var aeds = vm.AedsViewModel?.Value?.Aeds;
			if(aeds != null )
			{
				UpdatePins(aeds);
			}

			bool firstTime = true;
			vm.UserLocation.PropertyChanged += (sender, e) => 
			{
				if(e.PropertyName != "Value" || !firstTime)
					return;

				firstTime = false;

				// ユーザー位置へ
				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					vm.UserLocation.Value.Value, map.VisibleRegion.Radius));

				if(vm.SearchNearAedsCommand.CanExecute())
				{
					Task.Factory.StartNew(async () =>
					{
						await Task.Delay(TimeSpan.FromMilliseconds(1000));
						Device.BeginInvokeOnMainThread(() =>
							vm.SearchNearAedsCommand.Execute(map));
					});
				}
			};
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

		protected override bool OnBackButtonPressed()
		{
			if(flyOut.IsVisible)
			{
				HideFlyout();
				return true;
			}
				
			return base.OnBackButtonPressed();
		}

		private void MoveToCurrentPositionButtonClicked(object sender, EventArgs e)
		{
			if(!mapExBehavior.UserLocation.HasValue)
				return;

			var userLocation = mapExBehavior.UserLocation;
			map.MoveToRegion(MapSpan.FromCenterAndRadius(
				userLocation.Value, map.VisibleRegion.Radius));
		}

		private void FlyOutButtonClicked(object sender, EventArgs e)
		{
			#if DEBUG
			var position = mapExBehavior.UserLocation.HasValue
				? mapExBehavior.UserLocation.Value
				: new Position(0,0);
			radiusLabel.Text = Settings.RegionRadius.ToString() + Environment.NewLine +
				map.VisibleRegion.Radius.Meters + Environment.NewLine +
				"(" + position.Latitude + "," + position.Longitude + ")";
			#endif

			if(flyOut.IsVisible)
			{
				HideFlyout();
			}
			else
			{
				ShowFlyout();
			}
		}

		private async void HamburgerButtonClicked(object sender, EventArgs e)
		{
			var menuPage = new MenuPage();
			await Navigation.PushAsync(menuPage);
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

		private async void ShowFlyout()
		{
			if(!flyOut.IsVisible)
			{
				flyOut.TranslationX = 0;
				flyOut.TranslationY = flyOut.Height;
				await flyOut.TranslateTo(0, 0, 300);
				flyOut.IsVisible = !flyOut.IsVisible;
			}
		}

		private async void HideFlyout()
		{
			if(flyOut.IsVisible)
			{
				await flyOut.TranslateTo(0, flyOut.Height, 300);
				flyOut.IsVisible = !flyOut.IsVisible;
			}
		}
	}
}

