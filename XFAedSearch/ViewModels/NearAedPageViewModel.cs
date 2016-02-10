using System;
using System.ComponentModel;

using AedOpenDataApiWrapper;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XFAedSearch.ViewModels;
using System.Threading.Tasks;

using XFAedSearch.Models;

namespace XFAedSearch.ViewModels
{
	public class NearAedPageViewModel
	{
		public ReactiveProperty<AedsViewModel> AedsViewModel{ get; private set;}

		public ReactiveProperty<bool> IsUpdating { get; private set;}

		public ReactiveProperty<Position> UserLocation { get; private set;}

		public ReactiveCommand<Map> SearchNearAedsCommand { get; private set;}

		public NearAedPageViewModel ()
		{
			AedsViewModel = new ReactiveProperty<AedsViewModel>(new AedsViewModel());
			IsUpdating = new ReactiveProperty<bool>(false);
			UserLocation = new ReactiveProperty<Position>(new Position(Settings.RegionLatitude, Settings.RegionLongitude));
			SearchNearAedsCommand = IsUpdating.Select(x => !x).ToReactiveCommand<Map>();

			SearchNearAedsCommand.Subscribe(async map =>
			{
				if(map?.VisibleRegion != null)
				{
					var position = UserLocation.Value;
					Settings.RegionLatitude = position.Latitude;
					Settings.RegionLongitude = position.Longitude;
					Settings.RegionRadius = map.VisibleRegion.Radius.Meters;
				}

				await UpdateNearAeds().ConfigureAwait(false);
			});
		}

		public async Task UpdateNearAeds()
		{
			if(IsUpdating.Value)
				return;

			IsUpdating.Value = true;

			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			var radius = Settings.RegionRadius;
			var position = UserLocation.Value;

			Task<List<AedInfo>> nearestAedTask;
			Task<List<AedInfo>> nearAedsTask;
			try
			{
				using(nearestAedTask = new ApiClient().NearAedAsync(position.Latitude, position.Longitude))
				using(nearAedsTask = new ApiClient().AedSearchAsync(position.Latitude, position.Longitude,
					(int)(radius * 1.2)))
				{
					await Task.WhenAll(nearestAedTask, nearAedsTask).ConfigureAwait(false);
				}
			}
			catch(Exception exception)
			{
				IsUpdating.Value = false;

				Console.WriteLine(exception);
				Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
				Console.WriteLine(@"Message published: key=""/SearchNearAeds/?Result=Failed"":AedApiRequestFailedMessage");
				Device.BeginInvokeOnMainThread(() =>
				{
					MessagingCenter.Send(this, "/SearchNearAeds/?Result=Failed",
						"AedApiRequestFailedMessage");
				});
				return;
			}

			var newAeds = nearestAedTask.Result
				.Concat(nearAedsTask.Result)
				.GroupBy(aed => aed.Id)
				.Select(group => new AedViewModel(group.First()))
				.ToList();

			Device.BeginInvokeOnMainThread(() =>
			{
				AedsViewModel.Value.Aeds = newAeds;
			});

			IsUpdating.Value = false;
		}
	}
}

