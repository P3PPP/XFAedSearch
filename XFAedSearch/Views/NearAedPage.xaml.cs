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

			if(Device.OS == TargetPlatform.Android)
			{
				// AndroidはMapのロード完了前にRegion移動すると可視範囲(ズーム)が指定どおりにならない問題への対処
				mapExBehavior.MapLoaded += (sender, e) =>
				{
					// 前回マップに表示していた位置を復元はEffectでやる

					// 最初だけ周囲のAEDを検索するよ
					var vm = (BindingContext as NearAedPageViewModel);
					if(vm != null && vm.AedsViewModel.Value.Aeds == null ||
					   vm.AedsViewModel.Value.Aeds.Count == 0)
					{
						if(vm.SearchNearAedsCommand.CanExecute())
						{
							Task.Factory.StartNew(async () =>
							{
								await Task.Delay(TimeSpan.FromMilliseconds(1000));
								Device.BeginInvokeOnMainThread(() =>
									vm.SearchNearAedsCommand.Execute(map));
							});
						}
					}
				};
			}
			else
			{
				// 前回マップに表示していた位置を復元
				map.MoveToRegion(MapSpan.FromCenterAndRadius(
					new Position(Settings.RegionLatitude, Settings.RegionLongitude),
					Distance.FromMeters(Settings.RegionRadius)));
				
				mapExBehavior.MapLoaded += (sender, e) =>
				{
					// ユーザー位置へ
					var userLocation = mapExBehavior.UserLocation;
					map.MoveToRegion(MapSpan.FromCenterAndRadius(
						userLocation, map.VisibleRegion.Radius));

					// 最初だけ周囲のAEDを検索するよ
					var vm = (BindingContext as NearAedPageViewModel);
					if(vm != null && vm.AedsViewModel.Value.Aeds == null ||
						vm.AedsViewModel.Value.Aeds.Count == 0)
					{
						if(vm.SearchNearAedsCommand.CanExecute())
						{
							Task.Factory.StartNew(async () =>
							{
								await Task.Delay(TimeSpan.FromMilliseconds(1000));
								Device.BeginInvokeOnMainThread(() =>
									vm.SearchNearAedsCommand.Execute(map));
							});
						}
					}
				};
			}

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
			#if DEBUG
			radiusLabel.Text = Settings.RegionRadius.ToString() + Environment.NewLine +
				map.VisibleRegion.Radius.Meters;
			#endif

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
			Element ancestor = this.Parent;
			while(true)
			{
				if(ancestor is MasterDetailPage || ancestor == null)
				{
					break;
				}
				ancestor = ancestor.Parent;
			}

			if(ancestor != null)
			{
				(ancestor as MasterDetailPage).IsPresented = !(ancestor as MasterDetailPage).IsPresented;
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

//			if(Device.OS != TargetPlatform.Android)
//			{
//				// 最初だけ周囲のAEDを検索するよ
//				var vm = (BindingContext as NearAedPageViewModel);
//				if(vm != null && vm.AedsViewModel.Value.Aeds == null ||
//				  vm.AedsViewModel.Value.Aeds.Count == 0)
//				{
//					if(vm.SearchNearAedsCommand.CanExecute())
//					{
//						Task.Factory.StartNew(async () =>
//						{
//							await Task.Delay(TimeSpan.FromMilliseconds(1000));
//							Device.BeginInvokeOnMainThread(() =>
//							vm.SearchNearAedsCommand.Execute(map));
//						});
//					}
//				}
//			}
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

